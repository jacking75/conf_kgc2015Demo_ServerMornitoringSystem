// http 요청 클래스
public static class NetHttpRequester
{
    // 결과를 오브젝트로 반환한다
    public static RES_T RequestReturnObjet<REQ_T, RES_T>(string apiUrl, REQ_T requestPkt) where RES_T : IRES_REAL_DATA, new()
    {
        try
        {
            var content = CreateHttpJsonContentFromObject<REQ_T>(requestPkt);

            HttpClient httpClient = new HttpClient();

            var response = await httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode == false)
            {
                var returnPkt = new RES_T();
                returnPkt.SetResult(ERROR_CODE.FAIL_NETWORK_HTTP_REQUEST);
                return returnPkt;
            }

            var result = await response.Content.ReadAsStringAsync();
            var responsePkt = Jil.JSON.Deserialize<RES_T>(result);
            return responsePkt;
        }
        catch (Exception ex)
        {
            FileLogger.Exception(ex.Message);

            var returnPkt = new RES_T();
            returnPkt.SetResult(ERROR_CODE.EXCEPTION_HTTP_REQUEST);
            return returnPkt;
        }
    }

    // 결과를 문자열로 반환한다
    public static async Task<string> RequestReturnString<REQ_T>(string apiUrl, REQ_T requestPkt)
    {
        try
        {
            var content = CreateHttpJsonContentFromObject<REQ_T>(requestPkt);

            HttpClient httpClient = new HttpClient();

            var response = await httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode == false)
            {
                return "";
            }

            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
        catch (Exception ex)
        {
            FileLogger.Exception(ex.Message);
            return "";
        }
    }
    // 결과를 문자열로 반환한다. 요청할 때의 인자 값을 form 방식을 사용하는 경우
    public static async Task<string> ReturnString(string domain, string api, List<KeyValuePair<string, string>> keyValueList)
    {
        try
        {
            //http://stackoverflow.com/questions/15176538/net-httpclient-how-to-post-string-value
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(domain);
                var content = new FormUrlEncodedContent(keyValueList);
                var result = await client.PostAsync(api, content);

                if (result.IsSuccessStatusCode == false)
                {
                    return "";
                }
                else
                {
                    // 로그 남기기
                }

                string resultContent = await result.Content.ReadAsStringAsync();
                return resultContent;
            }
        }
        catch (Exception ex)
        {
            FileLogger.Exception(ex.Message);
            return "";
        }
    }

    // 요청 성공 여부만 반환. 리모트에서 데이터를 보내지 않는 경우.
    public static async Task<bool> RequestReturnNone<REQ_T>(string apiUrl, REQ_T requestPkt)
    {
        try
        {
            var content = CreateHttpJsonContentFromObject<REQ_T>(requestPkt);

            HttpClient httpClient = new HttpClient() { Timeout = TimeSpan.FromMilliseconds(1000) };

            var response = await httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode == false)
            {
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            FileLogger.Exception(ex.Message);
            return false;
        }
    }



    static HttpContent CreateHttpJsonContentFromObject<T>(T obj)
    {
        var jsonText = Jil.JSON.Serialize(obj);
        var utf8Bytes = Encoding.UTF8.GetBytes(jsonText);
        var content = new ByteArrayContent(utf8Bytes);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return content;
    }



}
