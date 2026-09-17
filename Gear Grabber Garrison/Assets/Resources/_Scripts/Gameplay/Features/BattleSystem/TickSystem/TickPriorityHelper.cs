namespace Gameplay.Features.Battlesystem
{
    public static class TickPriorityHelper
    {
        /// <summary>
        /// Конвертирует Team в TickPriority
        /// </summary>
        public static TickPriority FromTeam(Team team)
        {
            return team switch
            {
                Team.Player => TickPriority.Player,
                Team.Enemy => TickPriority.Enemy,
                _ => TickPriority.Default
            };
        }
    }
}
