using CMG.License.Shared.DataTypes;
using CMG.License.Shared.Helpers;
using System;
using System.Linq;

namespace CMG.License.Services.Impls
{
    internal static class RequestTimeProcessingService
    {
        //public static DateTime GetStrRequestTime(int checkOutKey, LogFile logFile)
        //{
        //    string strRequestTime = string.Empty;
        //    var checkOut = logFile.CheckOuts.ElementAt(checkOutKey);
        //    var checkOutTime = $"{logFile.Year}/{checkOut[CheckOut.mm_dd]} {checkOut[CheckOut.time]}";
        //    if (!logFile.Denials.Any())
        //        strRequestTime = checkOutTime;
        //    else
        //    {
        //        string productName = checkOut[CheckOut.product];
        //        string userName = checkOut[CheckOut.user];

        //        var userDenialsForProduct = logFile.Denials.Where(x =>
        //                              x.Value[Deny.user] == userName
        //                              && x.Value[Deny.product] == productName);

        //        if (userDenialsForProduct.Any())
        //        {
        //            var lastCheckOutBeforCurrForProd = logFile.CheckOuts.Where(x => x.Key < checkOutKey
        //                                            && x.Value[CheckOut.product] == productName)
        //                                            .OrderBy(x => x.Key).LastOrDefault();

        //            var firstDenialforUserAfterAbove = userDenialsForProduct.FirstOrDefault(x =>
        //                              x.Key > lastCheckOutBeforCurrForProd.Key &&
        //                              x.Key < checkOutKey).Value;

        //            if (firstDenialforUserAfterAbove != null)
        //                strRequestTime = $"{logFile.Year}/{firstDenialforUserAfterAbove?[Deny.mm_dd]} {firstDenialforUserAfterAbove?[Deny.time]}";
        //            else
        //                strRequestTime = checkOutTime;
        //        }
        //        else
        //            strRequestTime = checkOutTime;
        //    }

        //    return strRequestTime.GetFormattedDateTime();
        //}
    }
}