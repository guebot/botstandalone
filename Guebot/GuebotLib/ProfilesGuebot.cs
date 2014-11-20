using GuebotLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guebot
{
    public class ProfilesGuebot
    {
        public static List<ProfileEntity> GetProfiles()
        {
            List<ProfileEntity> profiles = new List<ProfileEntity>();

            // Huevo AAA
            profiles.Add(
                new ProfileEntity()
                {
                    Name = "AAA",
                    Arm = new GuebotComponentEntity() { Id = 1, ActualValue = 0, MaxValue = 1000, MinValue = 0, StepMovement = 50 },
                    Hand = new GuebotComponentEntity() { Id = 2, ActualValue = 0, MaxValue = 1000, MinValue = 0, StepMovement = 50 }
                });

            // Otro Huevo
            profiles.Add(
                new ProfileEntity()
                {
                    Name = "Huevo 2",
                    Arm = new GuebotComponentEntity() { Id = 1, ActualValue = 0, MaxValue = 800, MinValue = 0, StepMovement = 50 },
                    Hand = new GuebotComponentEntity() { Id = 2, ActualValue = 0, MaxValue = 500, MinValue = 0, StepMovement = 50 }
                });

            return profiles;
        }
    }
}
