namespace ALIVEMusicAPI.Models.Entities;


    public class SubscriptionPlan
    {
        public int SubscriptionPlanID { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string Duration { get; set; } = null!; // Weekly, Monthly, Yearly
                                                    
    }

