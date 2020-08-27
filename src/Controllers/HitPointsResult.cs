using HitPointsTracker.Models;

namespace HitPointsTracker.Controllers
{
    public class HitPointsResult
    {
        public long Id { get; }

        public string Name { get; }

        public int MaxHitPoints { get; }

        public HitPointsSnapshot? Previous { get; private set; }

        public HitPointsSnapshot Current { get; private set; }

        public void Update(Character character)
        {
            Previous = Current;
            Current = new HitPointsSnapshot(character);
        }

        public HitPointsResult(Character character)
        {
            Id = character.Id;
            Name = character.Name;
            MaxHitPoints = character.MaxHitPoints;
            Current = new HitPointsSnapshot(character);
        }

        public class HitPointsSnapshot
        {
            public int HitPoints { get; }
            public int TempHitPoints { get; }

            public HitPointsSnapshot(Character character)
            {
                HitPoints = character.CurHitPoints;
                TempHitPoints = character.TempHitPoints;
            }
        }
    }
}
