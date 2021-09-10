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


        public RestClient Client = new RestClient();
        public RestRequest ClientRequest = new RestRequest();

        public string Token { get; set; }

        public QingLongController(ILogger<QingLongController> logger, IConfigServices config, IHttpContextAccessor accessor)
        {
            _logger = logger;
            _config = config;
            _accessor = accessor;

            Client.Timeout = -1;
            ClientRequest.AddHeader("Content-Type", "application/json");
        }

        #region 青龙

        /// <summary>
        /// 新增环境变量
        /// </summary>
        /// <param name="envs">环境变量数组</param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ContentResultModel> env(List<EnvModel> envs)
        {
            var Results = new Dictionary<string, object>();

            //获取Token
            await GetToken();
            if (string.IsNullOrWhiteSpace(Token))
            {
                return new ContentResultModel()
                {
                    code = 400,
                    msg = "请检查服务端配置文件是否正确"
                };
            }
            ClientRequest.AddHeader("Authorization", $"{Token}");

            //添加环境变量
            foreach (var item in envs)
            {
                item.remarks = "KingFeng:" + item.remarks;
            }

            var body = envs.ToJson();

            var url = $"{_config.config.QL_URL}open/envs?t={ExtensionsMethod.GetTimeStamp()}";
            Client.BaseUrl = new Uri(url);
            ClientRequest.Method = Method.POST;

            ClientRequest.AddParameter("application/json", body, ParameterType.RequestBody);

            var Content = ((await Client.ExecuteAsync(ClientRequest)).Content).ToJobj();
            if (Content.ContainsKey("code") || Content["code"].ToString() == "200")
            {
                if (!string.IsNullOrWhiteSpace(_config.config.WsKeyTaskFullName))
                {
                    ClientRequest = new RestRequest();
                    //执行wskey转换
                    await task(_config.config.WsKeyTaskFullName, _config.config.SecretKey);
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
        public async Task<ContentResultModel> updateEnv([Required] string uid, [Required] string wskey, string remarks = "")
        {
            var Results = new Dictionary<string, object>();

            await GetToken();
            if (string.IsNullOrWhiteSpace(Token))
            {
                return new ContentResultModel()
                {
                    code = 400,
                    msg = "请检查服务端配置文件是否正确"
                };
            }
            ClientRequest.AddHeader("Authorization", $"{Token}");

            if (string.IsNullOrWhiteSpace(uid) && string.IsNullOrWhiteSpace(wskey))
            {
                return new ContentResultModel()
                {
                    code = 400,
                    msg = "uid或wskey不能为空!"
                };
            }

            var envs = await this.envs(_config.config.SecretKey);

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

                var Url = $"{_config.config.QL_URL}open/envs?t={ExtensionsMethod.GetTimeStamp()}";
                Client.BaseUrl = new Uri(Url);
                ClientRequest.Method = Method.PUT;
                ClientRequest.AddParameter("application/json", env.ToJson(), ParameterType.RequestBody);

                var Content = ((await Client.ExecuteAsync(ClientRequest)).Content).ToJobj();
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
        public async Task<ContentResultModel> deleteEnv([Required] string uid)
        {
            var Results = new Dictionary<string, object>();

            await GetToken();
            if (string.IsNullOrWhiteSpace(Token))
            {
                return new ContentResultModel()
                {
                    code = 400,
                    msg = "请检查服务端配置文件是否正确"
                };
            }
            ClientRequest.AddHeader("Authorization", $"{Token}");

            if (string.IsNullOrWhiteSpace(uid))
            {
                return new ContentResultModel()
                {
                    code = 400,
                    msg = "uid不能为空!"
                };
            }

            var envs = await this.envs(_config.config.SecretKey);

            var data = envs.data.ToJson().JsonTo<List<EnvModel2>>();

            data = data.Where(i => i._id.Contains(uid)).ToList();

            if (data != null && data.Count != 0)
            {
                var ids = new List<string>();
                ids.Add(data[0]._id);

                var Url = $"{_config.config.QL_URL}open/envs?t={ExtensionsMethod.GetTimeStamp()}";
                Client.BaseUrl = new Uri(Url);
                ClientRequest.Method = Method.DELETE;
                ClientRequest.AddParameter("application/json", ids.ToJson(), ParameterType.RequestBody);

                var Content = ((await Client.ExecuteAsync(ClientRequest)).Content).ToJobj();
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
        /// 用户是否存在
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ContentResultModel> exitst([Required] string uid)
        {
            var Results = new Dictionary<string, object>();

            await GetToken();
            if (string.IsNullOrWhiteSpace(Token))
            {
                return new ContentResultModel()
                {
                    code = 400,
                    msg = "请检查服务端配置文件是否正确"
                };
            }
            ClientRequest.AddHeader("Authorization", $"{Token}");

            var Url = $"{_config.config.QL_URL}open/envs?t={ExtensionsMethod.GetTimeStamp()}";
            Client.BaseUrl = new Uri(Url);
            ClientRequest.Method = Method.GET;

            var Content = ((await Client.ExecuteAsync(ClientRequest)).Content).ToJobj();
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
        /// 获取所有环境变量
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        private async Task<ContentResultModel> envs(string key, string serach = "", SerachType serachType = SerachType.Name)
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

            //await GetToken();
            //if (string.IsNullOrWhiteSpace(Token))
            //{
            //    return new ContentResultModel()
            //    {
            //        code = 400,
            //        msg = "请检查服务端配置文件是否正确"
            //    };
            //}
            ////ClientRequest.AddHeader("Authorization", $"{Token}");

            var Url = $"{_config.config.QL_URL}open/envs?t={ExtensionsMethod.GetTimeStamp()}";
            Client.BaseUrl = new Uri(Url);
            ClientRequest.Method = Method.GET;

            var Content = ((await Client.ExecuteAsync(ClientRequest)).Content).ToJobj();
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
        public async Task<ContentResultModel> task(string taskName, [FromQuery] string key)
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

            var Results = new Dictionary<string, object>();
            var TaskID = new List<string>();

            await GetToken();
            if (string.IsNullOrWhiteSpace(Token))
            {
                return new ContentResultModel()
                {
                    code = 400,
                    msg = "请检查服务端配置文件是否正确"
                };
            }
            ClientRequest.AddHeader("Authorization", $"{Token}");

            if (taskName == "ws")
            {
                taskName = _config.config.WsKeyTaskFullName;
            }

            Client.BaseUrl = new Uri($"{_config.config.QL_URL}open/crons?searchValue={taskName}&t={ExtensionsMethod.GetTimeStamp()}");
            ClientRequest.Method = Method.GET;
            var Content = ((await Client.ExecuteAsync(ClientRequest)).Content).ToJobj();
            if (Content.ContainsKey("code") || Content["code"].ToString() == "200")
            {
                var Data = JArray.Parse(Content["data"].ToString());
                if (Data.Count != 0)
                {
                    if (Data[0]["name"]?.ToString() != taskName)
                    {
                        return new ContentResultModel()
                        {
                            code = 400,
                            msg = "没有完全符合名称的任务"
                        };
                    }
                    TaskID.Add(Data[0]["_id"]?.ToString());
                }
            }

            //Body.Add(task_id);
            var url = $"{_config.config.QL_URL}open/crons/run?t={ExtensionsMethod.GetTimeStamp()}";

            Client.BaseUrl = new Uri(url);
            ClientRequest.Method = Method.PUT;
            ClientRequest.AddParameter("application/json", TaskID.ToJson(), ParameterType.RequestBody);


            Content = ((await Client.ExecuteAsync(ClientRequest)).Content).ToJobj();
            if (TaskID.Count != 0)
            {
                if (Content.ContainsKey("code") || Content["code"].ToString() == "200")
                {
                    _logger.LogInformation($"执行任务:{taskName}成功");

                    return new ContentResultModel()
                    {
                        code = 200,
                        msg = "执行成功"
                    };
                }
            }
            else
            {
                return new ContentResultModel()
                {
                    code = 400,
                    msg = "任务不存在"
                };
            }

            return new ContentResultModel()
            {
                code = 400,
                msg = Content.ToString()
            };
        }

        /// <summary>
        /// 任务日志
        /// </summary>
        /// <param name="taskName">任务名称</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ContentResultModel> log(string taskName, [FromQuery] string key)
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

            if (taskName == "ws")
            {
                taskName = _config.config.WsKeyTaskFullName;
            }

            if (string.IsNullOrWhiteSpace(taskName))
            {
                return new ContentResultModel()
                {
                    code = 400,
                    msg = "执行任务名不能为空"
                };
            }



            var Results = new Dictionary<string, object>();
            var TaskID = new List<string>();

            await GetToken();
            if (string.IsNullOrWhiteSpace(Token))
            {
                return new ContentResultModel()
                {
                    code = 400,
                    msg = "请检查服务端配置文件是否正确"
                };
            }
            ClientRequest.AddHeader("Authorization", $"{Token}");

            Client.BaseUrl = new Uri($"{_config.config.QL_URL}open/crons?searchValue={taskName}&t={ExtensionsMethod.GetTimeStamp()}");
            ClientRequest.Method = Method.GET;
            var Content = ((await Client.ExecuteAsync(ClientRequest)).Content).ToJobj();
            if (Content.ContainsKey("code") || Content["code"].ToString() == "200")
            {
                var Data = JArray.Parse(Content["data"].ToString());
                if (Data.Count != 0)
                {
                    if (Data[0]["name"]?.ToString() != taskName)
                    {
                        return new ContentResultModel()
                        {
                            code = 400,
                            msg = "没有完全符合名称的任务"
                        };
                    }
                    TaskID.Add(Data[0]["_id"]?.ToString());
                }
            }

            var url = $"{_config.config.QL_URL}open/crons/{TaskID[0]}/log?t={ExtensionsMethod.GetTimeStamp()}";

            Client.BaseUrl = new Uri(url);
            ClientRequest.Method = Method.GET;
            ClientRequest.AddParameter("application/json", TaskID.ToJson(), ParameterType.RequestBody);


            Content = ((await Client.ExecuteAsync(ClientRequest)).Content).ToJobj();
            if (TaskID.Count != 0)
            {
                if (Content.ContainsKey("code") || Content["code"].ToString() == "200")
                {
                    //_logger.LogInformation(Results.ToJson());

                    var Log = Content["data"]?.ToString();

                    return new ContentResultModel()
                    {
                        code = 200,
                        data = Log
                    };
                }
            }
            else
            {
                return new ContentResultModel()
                {
                    code = 400,
                    msg = "任务不存在"
                };
            }

            return new ContentResultModel()
            {
                code = 400,
                msg = Content.ToString()
            };
        }

        /// <summary>
        /// 获取青龙Token
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetToken()
        {
            if (_config.config.QL_URL != null && _config.config.QL_Client_ID != null && _config.config.QL_Client_Secret != null)
            {
                var url = $"{_config.config.QL_URL}open/auth/token?client_id={_config.config.QL_Client_ID}&client_secret={_config.config.QL_Client_Secret}";
                Client.BaseUrl = new Uri(url);
                ClientRequest.Method = Method.GET;

                var Content = ((await Client.ExecuteAsync(ClientRequest)).Content).ToJobj();
                if (Content.ContainsKey("code") && Content["code"].ToString() == "200")
                {
                    var token = Content["data"]["token"]?.ToString();
                    Token = "Bearer " + token;
                    return true;
                }
                else
                {
                    _logger.LogError("请检查服务端青龙配置项是否正确");
                    return false;
                }
            }
            else
            {
                _logger.LogError("请检查服务端青龙配置文件格式是否正确");
                return false;
            }
        }
        #endregion

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

            return new ContentResultModel()
            {
                code = 200,
                data= Data
            };
        }

        /// <summary>
        /// 修改SecretKey
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public ContentResultModel updateSecretKey(string oldkey,string newkey) 
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
                    msg="没权限操作"
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
