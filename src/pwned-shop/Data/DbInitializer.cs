using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Globalization;
using pwned_shop.Models;

namespace pwned_shop.Data
{
    public static class DbInitializer
    {
        public static void Initialize(PwnedShopDb db)
        {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            if (db.Users.Any())
                return;

            // populate Users table using data from csv/UserProfile.csv
            var rows = ReadCsv("Data/csv/UserProfile.csv");
            string dateFormat = "d/M/yyyy";
            for (int i = 1; i < rows.Count; i++)
            {
                var row = rows[i];
                User u = new User()
                {
                    Id = row[0],
                    FirstName = row[1],
                    LastName = row[2],
                    PasswordHash = row[3],
                    Salt = row[4],
                    DOB = DateTime.ParseExact(row[5], dateFormat, null),
                    Email = row[6],
                    Address = row[7]
                };

                db.Users.Add(u);
            }

            // populate Ratings table using data from csv/RatingESRB.csv
            rows = ReadCsv("Data/csv/RatingESRB.csv");
            for (int i = 1; i < rows.Count; i++)
            {
                var row = rows[i];
                Rating r = new Rating()
                {
                    ESRBRating = row[0],
                    RatingDesc = row[1],
                    AgeGroup = row[2],
                };

                db.Ratings.Add(r);
            }
            

            // populate Products table using data from csv/Product.csv
            rows = ReadCsv("Data/csv/Product.csv");
            for (int i = 1; i < rows.Count; i++)
            {
                var row = rows[i];
                Product p = new Product()
                {
                    Id = Convert.ToInt32(row[0]),
                    ProductName = row[1],
                    ProductDesc = row[2],
                    CatTags = row[3],
                    UnitPrice = (float)Convert.ToDouble(row[4]),
                    ESRBRating = row[5],
                    ImgURL = row[6],
                    Discount = (float)Convert.ToDouble(row[7])
                };

                db.Products.Add(p);
            }
            db.SaveChanges();

            // populate Discounts table using data from csv/Discount.csv
            rows = ReadCsv("Data/csv/Discount.csv");
            for (int i = 1; i < rows.Count; i++)
            {
                var row = rows[i];
                Discount d = new Discount()
                {
                    PromoCode = row[0],
                    StartDate = DateTime.ParseExact(row[1], dateFormat, null),
                    EndDate = DateTime.ParseExact(row[2], dateFormat, null),
                    Remarks = row[3],
                    DiscountPercent = (float)Convert.ToDouble(row[4])
                };

                db.Discounts.Add(d);
            }

            // populate Orders table using data from csv/Orders.csv
            rows = ReadCsv("Data/csv/Orders.csv");
            for (int i = 1; i < rows.Count; i++)
            {
                var row = rows[i];
                Order o = new Order()
                {
                    Id = row[0],
                    UserId = row[1],
                    Timestamp = DateTime.Parse(row[2], new CultureInfo("en-SG")),
                    PromoCode = row[3] == "" ? null : row[3]
                };
                db.Orders.Add(o);
            }

            // populate OrderDetails table using data from csv/OrderDetails.csv
            rows = ReadCsv("Data/csv/OrderDetails.csv");
            for (int i = 1; i < rows.Count; i++)
            {
                var row = rows[i];
                OrderDetail od = new OrderDetail()
                {
                    OrderId = row[0],
                    ProductId = Convert.ToInt32(row[1]),
                    ActivationCode = row[2],
                    GiftTo = row[3]
                };

                db.OrderDetails.Add(od);
            }

            // populate Reviews table using data from csv/UserReviews.csv
            rows = ReadCsv("Data/csv/UserReviews.csv");
            for (int i = 1; i < rows.Count; i++)
            {
                var row = rows[i];
                Review r = new Review()
                {
                    UserId = row[0],
                    ProductId = Convert.ToInt32(row[1]),
                    ReviewDate = DateTime.ParseExact(row[2], dateFormat, null),
                    Content = row[3],
                    StarAssigned = Convert.ToInt32(row[4])
                };

                db.Reviews.Add(r);
            }
            db.SaveChanges();
        }

        public static List<string[]> ReadCsv(string path)
        {
            List<string[]> rows = new List<string[]>();
            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

            using (var reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    var row = reader.ReadLine();
                    var values = CSVParser.Split(row);

                    // clean up the fields (remove " and leading spaces)
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = values[i].TrimStart(' ', '"');
                        values[i] = values[i].TrimEnd('"');
                    }

                    rows.Add(values);
                }
            }

            return rows;
        }
    }
}
