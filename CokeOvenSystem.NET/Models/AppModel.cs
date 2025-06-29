namespace CokeOvenSystem.Models
{
    public static class AppModel
    {
        public static readonly int[] OvenNumbers = { 1, 2, 3 };

        public static readonly string[] TimePoints =
        {
            "02:00", "06:00", "10:00", "14:00", "18:00", "22:00"
        };

        public static readonly string[] Chambers1 =
        {
            "1#", "2#", "3#", "4#", "5#", "6#", "7#", "8#", "9#", "11#", "12#", "13#", "14#", "15#",
            "16#", "17#", "18#", "19#", "21#", "22#", "23#", "24#", "25#", "26#", "27#", "28#", "29#",
            "31#", "32#", "33#", "34#", "35#", "36#", "37#", "38#", "39#", "41#", "42#", "43#", "44#",
            "45#", "46#", "47#", "48#", "49#", "51#", "52#", "53#", "54#", "55#", "56#", "57#", "58#",
            "59#", "61#", "62#", "63#", "64#", "65#", "66#", "67#", "68#", "69#", "71#", "72#", "73#", "74#", "75#"
        };

        public static readonly string[] Chambers2 =
        {
            "76#", "77#", "78#", "79#", "81#", "82#", "83#", "84#", "85#", "86#", "87#", "88#", "89#",
            "91#", "92#", "93#", "94#", "95#", "96#", "97#", "98#", "99#", "101#", "102#", "103#", "104#",
            "105#", "106#", "107#", "108#", "109#", "111#", "112#", "113#", "114#", "115#", "116#", "117#",
            "118#", "119#", "121#", "122#", "123#", "124#", "125#", "126#", "127#", "128#", "129#", "131#",
            "132#", "133#", "134#", "135#", "136#", "137#", "138#", "139#", "141#", "142#", "143#", "144#",
            "145#", "146#", "147#", "148#", "149#", "151#"
        };

        public static readonly string[] Chambers3 =
        {
            "1#", "2#", "3#", "4#", "5#", "6#", "7#", "8#", "9#", "11#", "12#", "13#", "14#", "15#",
            "16#", "17#", "18#", "19#", "21#", "22#", "23#", "24#", "25#", "26#", "27#", "28#", "29#",
            "31#", "32#", "33#", "34#", "35#", "36#", "37#", "38#", "39#", "41#", "42#", "43#", "44#",
            "45#", "46#", "47#", "48#", "49#", "51#", "52#", "53#", "54#", "55#", "56#", "57#", "58#",
            "59#", "61#", "62#", "63#", "64#", "65#", "66#", "67#", "68#", "69#", "71#", "72#", "73#", "74#", "75#"
        };

        public static string[] GetChambersForOven(int ovenNumber)
        {
            return ovenNumber switch
            {
                1 => Chambers1,
                2 => Chambers2,
                3 => Chambers3,
                _ => new string[0]
            };
        }

        public static readonly string[] Sequence1 = GenerateSequence(Chambers1);
        public static readonly string[] Sequence2 = GenerateSequence(Chambers2);
        public static readonly string[] Sequence3 = GenerateSequence(Chambers3);

        // 获取焦炉的 9-2 串序顺序表
        public static string[] GetSequenceForOven(int ovenNumber)
        {
            return ovenNumber switch
            {
                1 => Sequence1,
                2 => Sequence2,
                3 => Sequence3,
                _ => new string[0]
            };
        }

        private static string[] GenerateSequence(string[] chambers)
        {
            int total = chambers.Length;
            if (total == 0) return new string[0];

            List<string> sequence = new List<string>(total);

            int current = 0;
            while (sequence.Count < total)
            {

                // 从起始点开始，以 9 为步长遍历
                while (current < total)
                {
                    sequence.Add(chambers[current]);
                    current += 9;
                }

                // 超出编号范围，则回到开始并推进 2 个炭化室
                current = current % 9 + 2;

                // 若最开始进 2 超出 9，则再次取模
                if (current > 9)
                    current %= 9;
            }

            return sequence.ToArray();
        }
    }
}