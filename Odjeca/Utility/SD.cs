using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odjeca.Models;

namespace Odjeca.Utility
{
    public static class SD
    {
        public const string DefaultClothesImage = "defaultClothes.png";
        public const string ManagerUser = "Manager";
        public const string StorageUser = "StorageUser";
        public const string FrontDeskUser = "FrontDesk";
        public const string CustomerEndUser = "Customer";
        public const string ssCouponCode = "ssCouponCode";
        public const string ssShoppingCartCount = "ssCartcount";

        public const string StatusSubmitted = "Submitted";
        public const string StatusInProcess = "Being Prepared";
        public const string StatusReady = "Ready for Pickup";
        public const string StatusCompleted = "Completed";
        public const string StatusCancelled = "Cancelled";

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusRejected = "Rejected";

        public static string ConvertToRawHtml(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        public static double DiscountedPrice(Discount couponFromDb, double OriginalOrderTotal)
        {
            if (couponFromDb == null)
            {
                return OriginalOrderTotal;
            }
            else
            {
                if (couponFromDb.MinimumAmmount > OriginalOrderTotal)
                {
                    return OriginalOrderTotal;
                }
                else
                {
                    //everything is valid
                    if (Convert.ToInt32(couponFromDb.DiscountType) == (int)Discount.EDiscountType.Dollar)
                    {
                        //$10 off $100
                        return Math.Round(OriginalOrderTotal - couponFromDb.DiscountAmmount, 2);
                    }
                    if (Convert.ToInt32(couponFromDb.DiscountType) == (int)Discount.EDiscountType.Percent)
                    {
                        //10% off $100
                        return Math.Round(OriginalOrderTotal - (OriginalOrderTotal * couponFromDb.DiscountAmmount / 100), 2);
                    }
                }
            }
            return OriginalOrderTotal;
        }
    }
}
