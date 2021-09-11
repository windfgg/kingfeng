using KingFeng.Models;
using KingFeng.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KingFeng.Controllers
{
    [ApiController]
    [Route("api/[action]")]
    public class QingLongController : ControllerBase
    {
        public readonly ILogger<QingLongController> _logger;
        public readonly IConfigServices _config;
        private readonly IHttpContextAccessor _accessor;

        public QingLongController(ILogger<QingLongController> logger, IConfigServices config, IHttpContextAccessor accessor)
        {
            _logger = logger;
            _config = config;
            _accessor = accessor;
        }

        #region 青龙

        /// <summary>
        /// 新增环境变量
        /// </summary>
        /// <param name="envs">环境变量数组</param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ContentResultModel> env([Required] string ql_url, [Required] List<EnvModel> envs)
        {
            var Results = new Dictionary<string, object>();

            //获取Token
            var ServerState=await CheckServer(ql_url);
            if (string.IsNullOrWhiteSpace(ServerState.token))
            {
                return new ContentResultModel()
                {
                    code = 400
                };
            }

            Requset requset = new Requset();
            var Uri = new Uri($"{ql_url}open/envs?t={ExtensionsMethod.GetTimeStamp()}");
            var headers=new Dictionary<string, string>();
            var parameters = new List<Parameter>();
    
            headers.Add("Authorization", $"{ServerState.token}");

            var body = envs.ToJson();

            parameters.Add(new Parameter("application/json", body, ParameterType.RequestBody));

            var Content = await requset.HttpRequset(Uri, Method.POST, headers, parameters);
            if (Content.ContainsKey("code") && Content["code"].ToString() == "200")
            {
                if (!string.IsNullOrWhiteSpace(_config.config.WsKeyTaskFullName))
                {
                    //执行wskey转换
                    await task(ql_url, _config.config.WsKeyTaskFullName, _config.config.SecretKey);
                    _logger.LogInformation($"添加环境变量{envs[0].ToJson()} 已自动执行wskey转换");
                }

                var ContentData = Content["data"];

                var ids = new List<string>();
                foreach (var item in ContentData)
                {
                    var id = item["_id"].ToString();
                    ids.Add(id);
                }

                Results.Add("_id", ids);

                //_logger.LogInformation(Results.ToJson());

                return new ContentResultModel()
                {
                    code = 200,
                    data = Results
                };
            }

            return new ContentResultModel()
            {
                code = 400,
                msg = Content.ToString()
            };
        }

        /// <summary>
        /// 修改环境变量
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="wskey"></param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ContentResultModel> updateEnv([Required] string ql_url,[Required] string uid, [Required] string wskey, string remarks = "")
        {
            var Results = new Dictionary<string, object>();

            var ServerState = await CheckServer(ql_url);
            if (string.IsNullOrWhiteSpace(ServerState.token))
            {
                return new ContentResultModel()
                {
                    code = 400
                };
            }
            if (string.IsNullOrWhiteSpace(uid) && string.IsNullOrWhiteSpace(wskey))
            {
                return new ContentResultModel()
                {
                    code = 400,
                    msg = "uid或wskey不能为空!"
                };
            }


            var envs = await this.envs(ql_url,_config.config.SecretKey);
            var data = envs.data.ToJson().JsonTo<List<EnvModel2>>();
            data = data.Where(i => i._id.Contains(uid)).ToList();

            if (data != null && data.Count != 0)
            {
                var env = data[0];
                env.value = wskey;
                if (!string.IsNullOrWhiteSpace(remarks))
                {
                    env.remarks = remarks;
                }

                Requset requset = new Requset();
                var Uri = new Uri($"{ql_url}open/envs?t={ExtensionsMethod.GetTimeStamp()}");
                var headers = new Dictionary<string, string>();
                var parameters = new List<Parameter>();
                parameters.Add(new Parameter("application/json", env.ToJson(), ParameterType.RequestBody));
                headers.Add("Authorization", $"{ServerState.token}");

                var Content = await requset.HttpRequset(Uri, Method.PUT, headers, parameters);
        
                if (Content.ContainsKey("code") || Content["code"].ToString() == "200")
                {
                    _logger.LogInformation($"用户{env.name}修改环境变量成功");
                    return new ContentResultModel()
                    {
                        code = 200,
                        data = "修改成功"
                    };
                }

            }
            else
            {
                return new ContentResultModel()
                {
                    code = 400,
                    msg = "请检查uid是否正确"
                };
            }

            return new ContentResultModel()
            {
                code = 400,
                msg = "服务繁忙"
            };
        }

        /// <summary>
        /// 删除环境变量
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Produces("application/json")]
        public async Task<ContentResultModel> deleteEnv([Required] string ql_url, [Required] string uid)
        {
            var Results = new Dictionary<string, object>();

            var ServerState = await CheckServer(ql_url);
            if (string.IsNullOrWhiteSpace(ServerState.token))
            {
                return new ContentResultModel()
                {
                    code = 400
                };
            }

            if (string.IsNullOrWhiteSpace(uid))
            {
                return new ContentResultModel()
                {
                    code = 400,
                    msg = "uid不能为空!"
                };
            }

            var envs = await this.envs(ql_url, _config.config.SecretKey);

            var data = envs.data.ToJson().JsonTo<List<EnvModel2>>();

            data = data.Where(i => i._id.Contains(uid)).ToList();

            if (data != null && data.Count != 0)
            {
                var ids = new List<string>();
                ids.Add(data[0]._id);
                Requset requset = new Requset();
                var Uri = new Uri($"{ql_url}open/envs?t={ExtensionsMethod.GetTimeStamp()}");
                var headers = new Dictionary<string, string>();
                var parameters = new List<Parameter>();

                headers.Add("Authorization", $"{ServerState.token}");
                parameters.Add(new Parameter( "application/json", ids.ToJson(), ParameterType.RequestBody));

                var Content = await requset.HttpRequset(Uri, Method.DELETE, headers, parameters);
                if (Content.ContainsKey("code") || Content["code"].ToString() == "200")
                {
                    return new ContentResultModel()
                    {
                        code = 200,
                        data = "删除成功"
                    };
                }

            }
            else
            {
                return new ContentResultModel()
                {
                    code = 400,
                    msg = "删除失败,uid不存在"
                };
            }

            return new ContentResultModel()
            {
                code = 400,
                msg = "服务繁忙"
            };
        }

        /// <summary>
        /// 获取所有环境变量
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        private async Task<ContentResultModel> envs([Required] string ql_url, [Required] string secretkey, string serach = "", SerachType serachType = SerachType.Name)
        {
            if (!string.IsNullOrWhiteSpace(secretkey))
            {
                if (_config.config.SecretKey != secretkey)
                {
                    return new ContentResultModel()
                    {
                        code = 401,
                        msg = "没权限操作"
                    };
                }
            }
            else
            {
                return new ContentResultModel()
                {
                    code = 401,
                    msg = "secretkey不能为空"
                };
            }

            var ServerState = await CheckServer(ql_url);
            if (string.IsNullOrWhiteSpace(ServerState.token))
            {
                return new ContentResultModel()
                {
                    code = 400
                };
            }

            Requset requset = new Requset();
            var Uri = new Uri($"{ql_url}open/envs?t={ExtensionsMethod.GetTimeStamp()}");
            var headers = new Dictionary<string, string>();
            var parameters = new List<Parameter>();
            headers.Add("Authorization", $"{ServerState.token}");

            var Content = await requset.HttpRequset(Uri, Method.GET, headers);
            if (Content.ContainsKey("code") || Content["code"].ToString() == "200")
            {
                var ContentData = Content["data"].ToString().JsonTo<List<EnvModel2>>();
                if (!string.IsNullOrWhiteSpace(serach))
                {
                    if (serachType == SerachType.Id)
                    {
                        ContentData = ContentData.Where(i => i._id.Contains(serach)).ToList();
                    }
                    else if (serachType == SerachType.Name)
                    {
                        ContentData = ContentData.Where(i => i.name.Contains(serach)).ToList();
                    }
                    else
                    {
                        ContentData = ContentData.Where(i => i.value.Contains(serach)).ToList();
                    }
                }
                //_logger.LogInformation(Results.ToJson());

                return new ContentResultModel()
                {
                    code = 200,
                    data = ContentData
                };
            }
            return new ContentResultModel()
            {
                code = 400,
                msg = Content.ToString()
            };
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="taskName">名称</param>
        /// <returns></returns>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ContentResultModel> task([Required] string ql_url, [Required] string taskName, [FromQuery] string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                if (_config.config.SecretKey != key)
                {
                    return new ContentResultModel()
                    {
                        code = 401,
                        msg = "没权限操作"
                    };
                }
            }
            else
            {
                return new ContentResultModel()
                {
                    code = 401,
                    msg = "没权限操作"
                };
            }

            if (string.IsNullOrWhiteSpace(taskName))
            {
                return new ContentResultModel()
                {
                    code = 400,
                    msg = "执行任务名不能为空"
                };
            }

            var ServerState = await CheckServer(ql_url);
            if (string.IsNullOrWhiteSpace(ServerState.token))
            {
                return new ContentResultModel()
                {
                    code = 400
                };
            }

            var taskData = await taskExitst(ql_url, taskName, ServerState.token);
            if (!taskData.Item1)
            {
                _logger.LogWarning($"来自{ql_url} 任务名{taskName}不存在");
                return new ContentResultModel()
                {
                    code = 400
                };
            }

            Requset requset = new Requset();
            var Uri = new Uri($"{ql_url}open/crons/run?t={ExtensionsMethod.GetTimeStamp()}");
            var headers = new Dictionary<string, string>();
            var parameters = new List<Parameter>();
            var TaskIdLsit = new List<string>();
            TaskIdLsit.Add(taskData.Item2);
            headers.Add("Authorization", $"{ServerState.token}");
            parameters.Add(new Parameter("application/json", TaskIdLsit.ToJson(), ParameterType.RequestBody));
       
            var Content = await requset.HttpRequset(Uri, Method.PUT, headers, parameters);

            if (TaskIdLsit.Count != 0)
            {
                if (Content.ContainsKey("code") && Content["code"].ToString() == "200")
                {
                    _logger.LogInformation($"来自{ql_url} 执行任务:{taskName}成功");

                    return new ContentResultModel()
                    {
                        code = 200,
                        msg = "执行成功"
                    };
                }
            }
        
            return new ContentResultModel()
            {
                code = 400,
                msg = Content.ToString()
            };
        }

        /// <summary>
        /// 任务是否存在
        /// </summary>
        /// <param name="ql_url"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        private async Task<(bool,string)> taskExitst([Required] string ql_url, [Required] string taskName, string token)
        {
            var ServerState = await CheckServer(ql_url);
            if (string.IsNullOrWhiteSpace(ServerState.token))
            {
                _logger.LogError("执行任务存在性判断 token获取失败");
                return (false,"");
            }

            Requset requset = new Requset();
            var Uri = new Uri($"{ql_url}open/crons?searchValue={taskName}&t={ExtensionsMethod.GetTimeStamp()}");
            var headers = new Dictionary<string, string>();
            var parameters = new List<Parameter>();

            headers.Add("Authorization", token);

            var TaskID = new List<string>();

            var Content = await requset.HttpRequset(Uri, Method.GET, headers, parameters);

            if (Content.ContainsKey("code") || Content["code"].ToString() == "200")
            {
                var Data = JArray.Parse(Content["data"].ToString());
                if (Data.Count != 0)
                {
                    TaskID.Add(Data[0]["_id"]?.ToString());
                    return (true, TaskID[0]);
                }
                else
                {
                    return (false, "");
                }
            }
            else
            {
                return (false, "");
            }
        }

        /// <summary>
        /// 检查青龙服务
        /// </summary>
        /// <param name="config">服务配置</param>
        /// <returns></returns>
        private async Task<(bool state, string token)> CheckServer(string ql_url)
        {
            if (string.IsNullOrWhiteSpace(ql_url))
            {
                return (false, "");
            }
            ConfigItemModel model;
            try
            {
                //判断是否找到对应的配置
                var modelList = _config.config.Servers.Where(i => i.QL_URL == ql_url).ToList();
                if (modelList == null && modelList?.Count <= 0)
                {
                    _logger.LogWarning($"未找到{ql_url}对应的配置");
                    return (false, "");
                }
                model = modelList[0];
            }
            catch
            {
                return (false, "");
            }

            //获取token
            if (model.QL_URL != null && model.QL_Client_ID != null && model.QL_Client_Secret != null && model.QL_Name != null)
            {
                Requset requset = new Requset();
                var Uri = new Uri($"{model.QL_URL}open/auth/token?client_id={model.QL_Client_ID}&client_secret={model.QL_Client_Secret}");
                var Content = await requset.HttpRequset(Uri, Method.GET);

                //判断获取token是否成功
                if (Content.ContainsKey("code") && Content["code"].ToString() == "200")
                {
                    var token = Content["data"]["token"]?.ToString();
                    token = "Bearer " + token;


                    //if (model.MaxCount >= wsKeyCount + pinKeyCount)
                    //{
                    //    _logger.LogWarning($"服务名称{model.QL_Name}wsck和pinck已经到达最大数量 已无法添加新的cookies");
                    //    return (false, "");
                    //}

                    return (true, token);
                }
                else
                {
                    _logger.LogError($"服务名称{model.QL_Name}登录错误:{Content["Content"]?.ToString()}");
                    return (false, "");
                }
            }
            else
            {
                _logger.LogError($"服务名称{model.QL_Name} 请检查配置是否正确");
                return (false, "");
            }
        }

        #endregion

        #region 配置

        /// <summary>
        /// 用户是否存在
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ContentResultModel> exitst([Required] string ql_url, [Required] string uid)
        {
            var Results = new Dictionary<string, object>();

            var ServerState = await CheckServer(ql_url);
            if (string.IsNullOrWhiteSpace(ServerState.token))
            {
                return new ContentResultModel()
                {
                    code = 400
                };
            }
            Requset requset = new Requset();
            var Uri = new Uri($"{ql_url}open/envs?t={ExtensionsMethod.GetTimeStamp()}");
            var headers = new Dictionary<string, string>();
            var parameters = new List<Parameter>();
            headers.Add("Authorization", $"{ServerState.token}");

            var Content = await requset.HttpRequset(Uri, Method.GET, headers, parameters);
            if (Content.ContainsKey("code") || Content["code"].ToString() == "200")
            {
                var ContentData = Content["data"].ToString().JsonTo<List<EnvModel2>>();
                if (!string.IsNullOrWhiteSpace(uid))
                {
                    var ContentData1 = ContentData.Where(i => i._id == uid).ToList();
                    if (ContentData1 != null && ContentData1.Count != 0)
                    {
                        return new ContentResultModel()
                        {
                            code = 200,
                            data = !ContentData1[0].status,
                            msg = ContentData1[0].timestamp
                        };
                    }
                    else
                    {
                        return new ContentResultModel()
                        {
                            code = 400
                        };
                    }
                }
                else
                {
                    return new ContentResultModel()
                    {
                        code = 400
                    };
                }

            }
            else
            {
                return new ContentResultModel()
                {
                    code = 400,
                    msg = "服务繁忙"
                };
            }
        }

        /// <summary>
        /// 判断是否管理员
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ContentResultModel admin(string key)
        {
            if (key == _config.config.SecretKey)
            {
                return new ContentResultModel()
                {
                    code = 200
                };
            }
            else
            {
                return new ContentResultModel()
                {
                    code = 400
                };
            }
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ContentResultModel config()
        {
            var Data = new Dictionary<string, object>();
            Data.Add("push", _config.config.PushImageUrl);
            Data.Add("course", _config.config.Course);
            Data.Add("notice", _config.config.Notice);

            return new ContentResultModel()
            {
                code = 200,
                data = Data
            };
        }

        /// <summary>
        /// 修改SecretKey
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public ContentResultModel updateSecretKey(string oldkey, string newkey)
        {
            if (string.IsNullOrWhiteSpace(oldkey) || string.IsNullOrWhiteSpace(newkey))
            {
                return new ContentResultModel()
                {
                    code = 400,
                    msg = "SecretKey不能为空"
                };
            }

            if (oldkey != _config.config.SecretKey)
            {
                return new ContentResultModel()
                {
                    code = 400,
                    msg = "没权限操作"
                };
            }

            if (oldkey == newkey)
            {
                return new ContentResultModel()
                {
                    code = 400,
                    msg = "新老SecretKey 不能相等"
                };
            }

            //密码复杂度正则表达式
            var regex = new Regex(@"
    (?=.*[0-9])                     #必须包含数字
    (?=.*[a-zA-Z])                  #必须包含小写或大写字母
    ", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            //校验密码是否符合
            if (!regex.IsMatch(newkey))
            {
                return new ContentResultModel()
                {
                    code = 400,
                    msg = "密码强度不够,必须包含数字和大小写字母"
                };
            }

            var config = _config.config;
            config.SecretKey = newkey;

            _config.UpdateConfig(config);

            return new ContentResultModel()
            {
                code = 200,
                msg = "修改成功"
            };
        }

        /// <summary>
        /// 获取青龙服务列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ContentResultModel> servers()
        {
            var list = new List<ConfigItemModel1>();
            foreach (var item in _config.config.Servers)
            {
                list.Add(new ConfigItemModel1()
                {
                    QL_Name = item.QL_Name,
                    QL_URL = item.QL_URL,
                    MaxCount = item.MaxCount,
                    CurrentCount = await GetCurrentCount(item)
                });
            }

            return new ContentResultModel()
            {
                code = 200,
                data = list
            };
        }

        /// <summary>
        /// 获取配置当前余量
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private async Task<int?> GetCurrentCount(ConfigItemModel config)
        {
            var envList = await envs(config.QL_URL, _config.config.SecretKey);
            var envData = envList.data.ToJson().JsonTo<List<EnvModel2>>();

            var wsKeyCount = envData.Where(i => i.name == "JD_WSCK").ToList()?.Count;
            var pinKeyCount = envData.Where(i => i.name == "JD_COOKIE").ToList()?.Count;

            var CurrentCount = wsKeyCount + pinKeyCount;

            return CurrentCount;
        }
        #endregion

    }

    /// <summary>
    /// 查询类型
    /// </summary>
    public enum SerachType
    {
        /// <summary>
        /// 名称
        /// </summary>
        Name,
        /// <summary>
        /// _id
        /// </summary>
        Id,
        /// <summary>
        /// 值
        /// </summary>
        Value,
        Remarks,
    }
}
