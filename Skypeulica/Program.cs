using System;

namespace Skypeulica
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // login
            var secToken = SkypeApi.GetSecToken("buildythebuilder@gmail.com", "BbBuilder22@.");
            var token = SkypeApi.GetSkypeToken(secToken);
            var contacts = SkypeApi.GetAllContacts(token.skypeid, token.Token);

            //var userID = "live:.cid.6f340b045df93b21";
            //var token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjVFODQ4MjE0Qzc3MDczQUU1QzJCREU1Q0NENTQ0ODlEREYyQzRDODQiLCJ4NXQiOiJYb1NDRk1kd2M2NWNLOTVjelZSSW5kOHNUSVEiLCJ0eXAiOiJKV1QifQ.eyJpYXQiOjE2ODU0Mzc2MDYsImV4cCI6MTY4NTUyNDAwNSwic2t5cGVpZCI6ImxpdmU6LmNpZC42ZjM0MGIwNDVkZjkzYjIxIiwic2NwIjo5NTYsImNzaSI6IjE2ODU0Mzc2MDUiLCJjaWQiOiI2ZjM0MGIwNDVkZjkzYjIxIiwiYWF0IjoxNjg1NDM3NjA1fQ.YP3OBhER-HRe-yDMiiRFvgZfGrVH15xsTJ_XHJ96Gv6WHANH_CStW9EdV85J808Juavz6Qo_SZOR7xgcUSnskwWZnrtreKSShY092WuHo2Q-u7MGWRlxPPyer4FzCv3CxR6XFASOjt8ZMayd-CuNH7H0pJVqQRITHDJJ2Mlmgfux5lcgmTBQHEiEZ3yVoiz3K7jBGHp3MMaPO20h57ptNj2Wcq_BZWzEQT5CkxGqsPbbGylpZrDItZVSpCT9GjWe0l2nOV2Dg737OqptkmMKuZnoUcptA0Qmow0npyjgy6tqIzmLnBWvw-e9C8YrhWfKvpkl3tywcX8rtfuhnTKMDw";

            //var contacts = SkypeApi.GetAllContacts(token);
            // 
        }
    }
}