using GymManagement.DAL.Data.Models.Enums;

namespace GymManagement.DAL.Data.Models
{
    public class Trainer : GymUser
    {
        public Speciality Speciality { get; set; }
        //change CreatedAt column name to HireDate

        #region Relationships

        public ICollection<Session> Sessions { get; set; } = new HashSet<Session>();

        #endregion
    }
}
