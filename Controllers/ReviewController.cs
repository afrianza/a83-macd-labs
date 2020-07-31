using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BooksCatalogue.Models;
using Microsoft.AspNetCore.Mvc;

namespace BooksCatalogue.Controllers
{
    public class ReviewController : Controller
    {
        private string apiEndpoint = "https://katalogbuku-api.azurewebsites.net/api/";
        private readonly HttpClient _client;
        HttpClientHandler clientHandler = new HttpClientHandler();
        public ReviewController() {
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            _client = new HttpClient(clientHandler);
        }

        // GET: Review/AddReview/2
        public async Task<IActionResult> AddReview(int? bookId)
        {
            if (bookId == null)
            {
                return NotFound();
            }

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiEndpoint + "books/" + bookId);

            HttpResponseMessage response = await client.SendAsync(request);

            switch(response.StatusCode)
            {
                case HttpStatusCode.OK:
                    string responseString = await response.Content.ReadAsStringAsync();
                    var review = JsonSerializer.Deserialize<Review>(responseString);

                    ViewData["BookId"] = bookId;
                    return View(review);
                case HttpStatusCode.NotFound:
                    return NotFound();
                default:
                    return ErrorAction("Error. Status code = " + response.StatusCode + ": " + response.ReasonPhrase);
            }
        }

        // TODO: Tambahkan fungsi ini untuk mengirimkan atau POST data review menuju API
        // POST: Review/AddReview
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int id, [Bind("Id,BookId,ReviewerName,Rating,Comment")] Review review)
        {
          
            {
                var httpContent = new[] {
                    new KeyValuePair<string, string>("id", review.Id.ToString()),
                    new KeyValuePair<string, string>("bookId", review.BookId.ToString()),
                    new KeyValuePair<string, string>("reviewerName", review.ReviewerName),
                    new KeyValuePair<string, string>("rating", review.Rating.ToString()),
                    new KeyValuePair<string, string>("comment", review.Comment),
                };

                HttpContent content = new FormUrlEncodedContent(httpContent);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, apiEndpoint +"review/");
                request.Content = content;

                HttpResponseMessage response = await _client.SendAsync(request);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.NoContent:
                    case HttpStatusCode.Created:
                        return RedirectToAction(nameof(AddReview));
                    default:
                        return ErrorAction("Error. Status code = " + response.StatusCode);
                }
            }
        }
        /*{
                MultipartFormDataContent content = new MultipartFormDataContent();

                content.Add(new StringContent(review.BookId.ToString()), "bookId");
                content.Add(new StringContent(review.ReviewerName), "reviewername");
                content.Add(new StringContent(review.Rating.ToString()), "rating");
                content.Add(new StringContent(review.Comment), "comment");

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiEndpoint + "review/");
                request.Content = content;
                HttpResponseMessage response = await _client.SendAsync(request);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.NoContent:
                    case HttpStatusCode.Created:
                        
                        return RedirectToAction(nameof(AddReview));
                    default:
                        return ErrorAction("Error. Status code = " + response.StatusCode + "; " + response.ReasonPhrase);
                }
        }*/
        
        private ActionResult ErrorAction(string message)
        {
            return new RedirectResult("/Home/Error?message=" + message);
        }
    }
}