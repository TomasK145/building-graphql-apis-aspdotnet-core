﻿using System.Threading.Tasks;
using CarvedRock.Web.Models;
using GraphQL.Client;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Newtonsoft.Json;

namespace CarvedRock.Web.Clients
{
    //implementacia existujuceho GraphQL clienta
    public class ProductGraphClient
    {
        private readonly GraphQLClient _client;

        public ProductGraphClient(GraphQLClient client)
        {
            _client = client;
        }

        public async Task<ProductModel> GetProduct(int id) //dopytovanie pouzitim GraphQL API
        {
            var query = new GraphQLRequest
            {
                Query = @" 
                query productQuery($productId: ID!)
                { product(id: $productId) 
                    { id name price rating photoFileName description stock introducedAt 
                      reviews { title review }
                    }
                }",
                Variables = new {productId = id}
            };
            var response = await _client.PostAsync(query);
            return response.GetDataFieldAs<ProductModel>("product"); //product reprezentuje root node odpovede 
        }

        public async Task<ProductReviewModel> AddReview(ProductReviewInputModel review) //vytvaranie itemov pouzitim GraphQL API
        {
            var query = new GraphQLRequest
            {
                Query = @" 
                mutation($review: reviewInput!)
                {
                    createReview(review: $review)
                    {
                        id
                    }
                }",
                Variables = new { review }
            };
            var response = await _client.PostAsync(query);
            return response.GetDataFieldAs<ProductReviewModel>("createReview");
        }

        public async Task SubscribeToUpdates()
        {
            var result = await _client.SendSubscribeAsync("subscription { reviewAdded { title productId } }");
            result.OnReceive += Receive;
        }

        private void Receive(GraphQLResponse resp)
        {
            var review = resp.Data["reviewAdded"];
        }
    }
}
