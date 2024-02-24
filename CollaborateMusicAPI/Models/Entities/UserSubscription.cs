using CollaborateMusicAPI.Contexts;

namespace ALIVEMusicAPI.Models.Entities;


    public class UserSubscription
    {
        public int UserSubscriptionID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid UserID { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;
        public int SubscriptionPlanID { get; set; }
        public virtual SubscriptionPlan SubscriptionPlan { get; set; } = null!;
        // Other properties...
    }

