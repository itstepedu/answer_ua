namespace AnswerUA.Utils
{
    public class Ids
    {
        public static class Target
        {
            public const int Vona = 1;
            public const int Vin = 2;
            public const int Dity = 3;
            public const int Dim = 4;
        }

        public static class PType
        {
            public const int Novynky = 1;
            public const int Odjag = 2;
            public const int Vzuttya = 3;
            public const int Aksesuary = 4;
            public const int Sport = 5;
            public const int Premium = 6;
            public const int Brendy = 7;
            public const int Rozprod = 8;
            public const int Sumochky = 9;
            public const int Okuljary = 10;
            public const int Zhurnal = 11;
            // 12–16 — домашні
        }
        public static string SlugFromTypeId(int productTypeId)
        {
            return productTypeId switch
            {
                PType.Novynky => "novynky",
                PType.Odjag => "odjag",
                PType.Vzuttya => "vzuttya",
                PType.Aksesuary => "aksesuary",
                PType.Sport => "sport",
                PType.Premium => "premium",
                PType.Brendy => "brendy",
               // PType.Rozprodazh => "rozprodazh",
               // PType.Sumochky => "sumochky",
               // PType.Okulyary => "okulyary",
               // PType.Zhurnaly => "zhurnaly",
               //PType.Vitalnya => "vitalnya-spalnya",
               //PType.KuhnyaBar => "kuhnya-bar",
               //PType.VannaKimnata => "vanna-kimnata",
               //PType.Lifestyle => "lifestyle",
               //PType.HomeSpa => "home-spa",
                _ => "odjag" // дефолт, щоб не впасти
            };
        }
        public static int TargetIdFromSlug(string slug) =>
            slug switch { "vona" => Target.Vona, "vin" => Target.Vin, "dity" => Target.Dity, "dim" => Target.Dim, _ => Target.Vona };

        public static string SlugFromTargetId(int id) =>
            id switch { 1 => "vona", 2 => "vin", 3 => "dity", 4 => "dim", _ => "vona" };
    }
}
