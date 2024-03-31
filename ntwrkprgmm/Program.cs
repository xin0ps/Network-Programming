using System.Net;
using System.Web;

using HttpListener listener = new HttpListener();

listener.Prefixes.Add("http://127.0.0.1:27001/");
listener.Prefixes.Add("http://127.0.0.1:27002/");
listener.Prefixes.Add("http://localhost:27003/");

listener.Start();

while (true)
{

    var context = listener.GetContext();

    _ = Task.Run(() =>
    {
        HttpListenerRequest? request = context.Request;

        HttpListenerResponse? response = context.Response;
        response.ContentType = "text/html";
        response.Headers.Add("Content-Type", "text/html");
        response.Headers.Add("Server", "Rasul");
        response.Headers.Add("Date", DateTime.Now.ToString());
        using var writer = new StreamWriter(response.OutputStream);

        var url = request.RawUrl;
        Console.WriteLine(url);
        var list = url?.Split('/').ToList();

        if (url == "/")
        {
            response.StatusCode = 404;

            var index = File.ReadAllText("Web/index.html");
            writer.Write(index);
        }

        else if (list.Count == 2)
        {
            if (!list[1].EndsWith(".html")) { list[1] += ".html"; }
            if (File.Exists($"Web\\{list[1]}"))
            {
                response.StatusCode = 200;
                var index = File.ReadAllText($"Web/{list[1]}");
                writer.Write(index);
            }
            else
            {
                response.StatusCode = 404;

                var index = File.ReadAllText("Web/404.html");
                writer.Write(index);
            }


        }


        else if (list.Count > 2)
        {
            string filepath = "";
            for (int i = 0; i < list.Count; i++)
            {
                if (i == 0 || i == list.Count - 1)
                {
                    filepath += list[i];

                }
                else { filepath += list[i] + "/"; }

            }
            if (File.Exists(filepath))
            {
                if (filepath.EndsWith("jpg"))
                {
                    response.StatusCode = 200;
                    response.ContentType = "image/jpg";
                    FileStream str = new FileStream(filepath, FileMode.Open, FileAccess.Read);
                    //var index = File.ReadAllBytes(filepath);
                    //writer.Write(index);
                    str.CopyTo(response.OutputStream);
                    str.Dispose();
                    response.OutputStream.Dispose();
                }
                if (filepath.EndsWith("txt"))
                {
                    response.StatusCode = 200;
                    response.ContentType = "text/txt";

                    var index = File.ReadAllText(filepath);
                    writer.Write(index);
                }


            }
            else
            {
                response.StatusCode = 404;

                var index = File.ReadAllText("Web/404.html");
                writer.Write(index);
            }
        }


    });

}