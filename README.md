# Jovemnf.WebAPI

Pacode .net core para usar os verbos HTTP.

## Modo de Usar

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
                using (WebAPI web = new WebAPI("https://jsonplaceholder.typicode.com/posts/1", 5000, WebAPI.MethodRequest.GET))
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