# Jovemnf.WebAPI

Pacode .net core para usar os verbos HTTP.

## Modo de Usar

### GET

Para fazer um Get Simples segue o exemplo abaixo

```csharp
using System;
using Jovemnf.WebAPI;

namespace TesteWebAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            Exec();
            Console.Read();
        }

        private static async void Exec() {
            string url = "https://jsonplaceholder.typicode.com/posts/1";
            using (WebAPI web = new WebAPI(url, 5000, WebAPI.MethodRequest.GET))
            {
                try {
                    var post = await web.SendAndGet<Post>();
                    Console.WriteLine("ID: " + post.id);
                    Console.WriteLine("USER-ID: " + post.userId);
                    Console.WriteLine("TITLE: " + post.title);
                    Console.WriteLine("BODY: " + post.body);
                } catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
        }
    }

    class Post {
        public int userId { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public string body { get; set; }
    }
}

```

### GET LIST

Buscando uma listagem de posts

```csharp
using System;
using System.Collections.Generic;
using Jovemnf.WebAPI;

namespace TesteWebAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            Exec();
            Console.Read();
        }

        private static async void Exec() {
            string url = "https://jsonplaceholder.typicode.com/posts";
            using (WebAPI web = new WebAPI(url, 5000, WebAPI.MethodRequest.GET))
            {
                try {
                    var posts = await web.SendAndGet<List<Post>>();
                    Console.WriteLine("COUNT: " + posts.Count);
                } catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
        }
    }

    class Post {
        public int userId { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public string body { get; set; }
    }
}
```

### POST

Utilizando a lib via POST

```csharp
using System;
using Jovemnf.WebAPI;

namespace TesteWebAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            var post = new Post
            {
                userId = 1,
                title = "titulo 1",
                body = "body 1"
            };

            Exec(post);
            Console.Read();
        }

        private static async void Exec(Post post) {
            string url = "https://jsonplaceholder.typicode.com/posts";
            using (WebAPI web = new WebAPI(url, 5000, WebAPI.MethodRequest.POST))
            {
                try {
                    web.SetJson(post);

                    var returned = await web.Send<Post>();
                    Console.WriteLine("ID: " + returned.id);
                    Console.WriteLine("USER-ID: " + returned.userId);
                    Console.WriteLine("TITLE: " + returned.title);
                    Console.WriteLine("BODY: " + returned.body);
                } catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
        }
    }

    class Post {
        public int userId { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public string body { get; set; }
    }
}
```