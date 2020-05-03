﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AAA.Core.Dto
{
    public static class LookupKey
    {
        public static class OrderType
        {
            public const int Single = 1;
            public const int Batch = 2;
        }

        public static class OrderTransactionType
        {
            public const int PurchaseOrder = 0;
            public const int SalesOrder = 1;
        }
    }
}
