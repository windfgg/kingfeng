using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace KingFeng
{
    public static class ExtensionsMethod
    {
        public static string Toyaml(this object obj)
        {
            string yaml = "";
            if (obj == null) return "";
            try
            {
                var serializer = new SerializerBuilder().Build();
                yaml = serializer.Serialize(obj);
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("yam序列错误" + ex.Message);
            }
            return yaml;
        }

        public static T YamlTo<T>(this string yamlStr)
        {
            var deserializer = new DeserializerBuilder().Build();

            var results = deserializer.Deserialize<T>(yamlStr);

            return results;
        }

        public static string ToJson(this object obj) => obj != null ? JsonConvert.SerializeObject(obj) : null;

        public static T JsonTo<T>(this string str) => str != "" ? JsonConvert.DeserializeObject<T>(str) : default;

        public static JObject ToJobj(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return default;
            }
            return JObject.Parse(str);
        }

        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        public static bool PasswordIsMathch(this string str)
        {
            var regex = new Regex(@"
    (?=.*[0-9])                     #必须包含数字
    (?=.*[a-zA-Z])                  #必须包含小写或大写字母
    (?=([\x21-\x7e]+)[^a-zA-Z0-9])  #必须包含特殊符号
    .{6,15}                         #至少6个字符，最多30个字符
    ", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

            //校验密码是否符合
            return regex.IsMatch(str);
        }
    }

    /// <summary>
    /// Json帮助类
    /// 使用前需引用开源项目类库：Newtonsoft.Json.dll
    /// </summary>
    public static class JsonHelper
    {

        public static string SerializeObjct(object obj)
        {
            if (obj == null) 
                return default;

            return JsonConvert.SerializeObject(obj);
        }

        public static T JsonConvertObject<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default;

            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 将JSON转数组
        /// 用法:jsonArr[0]["xxxx"]
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <returns></returns>
        public static JArray GetToJsonList(this string json)
        {
            JArray jsonArr = (JArray)JsonConvert.DeserializeObject(json);
            return jsonArr;
        }

        /// <summary>
        /// 解析JSON字符串生成对象实体
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <returns></returns>
        public static T DeserializeJsonToObject<T>(this string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object obj = serializer.Deserialize(new JsonTextReader(sr), typeof(T));
            T t = obj as T;
            return t;
        }

        /// <summary>
        /// 解析JSON数组生成对象实体集合
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json数组</param>
        /// <returns>对象实体集合</returns>
        public static List<T> DeserializeJsonToList<T>(this string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object obj = serializer.Deserialize(new JsonTextReader(sr), typeof(List<T>));
            List<T> list = obj as List<T>;
            return list;
        }

        /// <summary>
        /// 将DataTable转换成实体类
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static List<T> DtConvertToModel<T>(DataTable dt) where T : new()
        {
            List<T> ts = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                foreach (PropertyInfo pi in t.GetType().GetProperties())
                {
                    if (dt.Columns.Contains(pi.Name))
                    {
                        if (!pi.CanWrite) continue;
                        var value = dr[pi.Name];
                        if (value != DBNull.Value)
                        {
                            switch (pi.PropertyType.FullName)
                            {
                                case "System.Decimal":
                                    pi.SetValue(t, decimal.Parse(value.ToString()), null);
                                    break;
                                case "System.String":
                                    pi.SetValue(t, value.ToString(), null);
                                    break;
                                case "System.Int32":
                                    pi.SetValue(t, int.Parse(value.ToString()), null);
                                    break;
                                default:
                                    pi.SetValue(t, value, null);
                                    break;
                            }
                        }
                    }
                }
                ts.Add(t);
            }
            return ts;
        }
    }
}
