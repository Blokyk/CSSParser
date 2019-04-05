using System;
using System.IO;
using System.Collections.Generic;

using System.Net;
using System.Net.Http;

using System.Threading.Tasks;

using System.Linq;

using Newtonsoft.Json;

namespace cssparser {
    class Tests {
        public static async Task<bool> IsCSSValid(string path) {

            if (String.IsNullOrEmpty(path)) {
                throw new ArgumentException("Path cannot be empty", "path");
            }
            
            using (var httpClient = new HttpClient()) {
                Dictionary<string, dynamic> respBody;

                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://file.io/")) {
                    var multipartContent = new MultipartFormDataContent();
                    multipartContent.Add(new ByteArrayContent(File.ReadAllBytes(path)), "file", Path.GetFileName("test.txt"));
                    request.Content = multipartContent; 

                    var uploadResponse = await httpClient.SendAsync(request);

                    respBody = ParseJsonBody(uploadResponse);
                }

                //uploadResponseBody.ToList().ForEach((s) => Console.WriteLine(s));

                if (respBody["success"] == false) {
                    throw new WebException(respBody["message"], WebExceptionStatus.SendFailure);
                }

                // var validatorResponse = await httpClient.GetAsync("http://jigsaw.w3.org/css-validator/validator?text=" + respBody["link"] + "&warning=0&profile=css3");
                var validatorResponse = await httpClient.GetAsync("http://jigsaw.w3.org/css-validator/validator?uri=http://www.w3.org/&warning=0&profile=css3");
                validatorResponse.EnsureSuccessStatusCode();

                //File.WriteAllText(Directory.GetCurrentDirectory() + "/res/index.html", Task<string>.Run(() => validatorResponse.Content.ReadAsStringAsync()).Result);
                //ParseJsonBody(validatorResponse);//.ToList().ForEach((s) => Console.WriteLine(s.Value));
            }

            return true;
        }

        public static Dictionary<string, dynamic> ParseJsonBody(HttpResponseMessage response) {
            
            var responseBody = Task<string>.Run(() => response.Content.ReadAsStringAsync()).Result;

            return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(responseBody);
        }
    }
}