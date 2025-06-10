namespace CbcRoastersErp.Models
{
    public class RoastingProfiles
    {
        public int ProfileID { get; set; }
        public string ProfileName { get; set; }
        public string Description { get; set; }
        public int GreenCoffeeID { get; set; }
        public int RoastTime { get; set; }
        public string RoastLevel { get; set; } // e.g., Light, Medium, Dark
        public decimal Temperature { get; set; }
        public string GreenCoffeeName { get; set; } //Display name for the coffee
        public string Notes { get; set; }
    }
}
