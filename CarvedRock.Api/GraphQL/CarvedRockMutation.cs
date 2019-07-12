using CarvedRock.Api.Data.Entities;
using CarvedRock.Api.GraphQL.Types;
using CarvedRock.Api.Repositories;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarvedRock.Api.GraphQL
{
    public class CarvedRockMutation : ObjectGraphType
    {
        public CarvedRockMutation(ProductReviewRepository reviewRepository, ReviewMessageService messageService)
        {
            FieldAsync<ProductReviewType>( //ProductReviewType --> definuje ze tento objekt bude vrateny po tom ako bude mutation vykonana
                "createReview", //poziadavka na vytvorenie review
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<ProductReviewInputType>> { Name = "review" } //pozadovany argument
                ),
                resolve: async context =>
                {
                    var review = context.GetArgument<ProductReview>("review"); //ziskanie argumentu a resolvovanie do entity objektu
                    //return await context.TryAsyncResolve(async c => await reviewRepository.AddReview(review));
                    //vystup z AddReview je pouzitim "TryAsyncResolve" monitorovany (???) , ak je vytvorenie uspesne, review je navratene normalne
                    //ak sa vsak vyskytne exception pri volani repository, exception je catchnuta a pridana do ErrorListu ktory je vrateny klientovi aj ked je API nastavena pre not-expose exception

                    await reviewRepository.AddReview(review);
                    messageService.AddReviewAddedMessage(review); //po pridani review su subscriberi notifikovani
                    return review;
                }
            );
        }
    }
}
