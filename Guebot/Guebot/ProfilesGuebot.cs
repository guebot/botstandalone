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
        public static List<Profile> GetProfiles()
        {
            List<Profile> profiles = new List<Profile>();

            // Huevo AAA
            profiles.Add(
                new Profile()
                {
                    Name = "Huevo AAA",
                    Arm = new GuebotComponent() { Id = 1, ActualValue = 0, MaxValue = 1000, MinValue = 0, StepMovement = 50 },
                    Hand = new GuebotComponent() { Id = 2, ActualValue = 0, MaxValue = 1000, MinValue = 0, StepMovement = 50 }
                });

            // Otro Huevo
            profiles.Add(
                new Profile()
                {
                    Name = "Huevo 2",
                    Arm = new GuebotComponent() { Id = 1, ActualValue = 0, MaxValue = 800, MinValue = 0, StepMovement = 50 },
                    Hand = new GuebotComponent() { Id = 2, ActualValue = 0, MaxValue = 500, MinValue = 0, StepMovement = 50 }
                });

            return profiles;
        }
    }
}
