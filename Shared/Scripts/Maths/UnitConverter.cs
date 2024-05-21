namespace MustHave
{
    public static class UnitConverter
    {
        public static float InchToCm(float inches)
        {
            return inches * 2.54f;
        }

        public static float CmToInch(float centimeters)
        {
            return centimeters / 2.54f;
        }

        public static float FtToMeter(float feet)
        {
            return feet * 0.3048f;
        }

        public static float MeterToFt(float meters)
        {
            return meters / 0.3048f;
        }
    }
}
