﻿using Akka.Util;
using CMG.License.Shared.DataTypes;
using System;
using System.Linq;

namespace CMG.License.Services.Impls
{
    internal static class RequestTimeProcessingService
    {
        public static DateTime GetStrRequestTime(CheckOutDto checkOut, ConcurrentSet<DenyDto> denys, ConcurrentSet<CheckOutDto> checkOuts)
        {
            if (!denys.Any())
                return checkOut.TimeStamp;
            else
            {
                var userDenialsForProduct = denys.Where(x =>
                                      x.User == checkOut.User
                                      && x.Product == checkOut.Product);

                if (userDenialsForProduct.Any())
                {
                    var lastCheckOutBeforCurrForProd
                        = checkOuts.Where(x =>
                                    x.TimeStamp <= checkOut.TimeStamp
                                    && x.Product == checkOut.Product)
                                    .OrderBy(x => x.TimeStamp).LastOrDefault();

                    var firstDenialforUserAfterAbove
                        = userDenialsForProduct.FirstOrDefault(x =>
                                      x.TimeStamp >= lastCheckOutBeforCurrForProd.TimeStamp &&
                                      x.TimeStamp <= checkOut.TimeStamp);

                    if (firstDenialforUserAfterAbove.TimeStamp != default(DateTime))
                        return firstDenialforUserAfterAbove.TimeStamp;
                }
                else
                    return checkOut.TimeStamp;
            }

            return checkOut.TimeStamp;
        }
    }
}