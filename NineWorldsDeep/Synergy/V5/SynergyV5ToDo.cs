using System;

namespace NineWorldsDeep.Synergy.V5
{
    public class SynergyV5ToDo
    {
        public int ToDoId { get; set; }
        public DateTime? ActivatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public DateTime? ArchivedAt { get; private set; }

        /// <summary>
        /// 
        /// will resolve conflicts, newest date will always take precedence
        /// passing null values allowed as well to just set one or two
        /// null values always resolve to the non-null value(unless both null)
        /// 
        /// NOTE: THIS CONVERTS TO UTC, WHICH IS DEPENDENT ON DateTime.Kind
        /// This property defaults to local, so if you are passing a UTC time, make
        /// sure that the Kind is set to UTC or the conversion will
        /// be off. 
        /// 
        /// </summary>
        /// <param name="newActivatedAt"></param>
        /// <param name="newCompletedAt"></param>
        /// <param name="newArchivedAt"></param>
        public void SetTimeStamps(DateTime? newActivatedAt, 
                                  DateTime? newCompletedAt, 
                                  DateTime? newArchivedAt)
        {
            if (newActivatedAt != null)
            {
                if (newActivatedAt.Value.Kind != DateTimeKind.Utc)
                {
                    newActivatedAt = newActivatedAt.Value.ToUniversalTime();
                }

                if (ActivatedAt == null ||
                   DateTime.Compare(ActivatedAt.Value, newActivatedAt.Value) < 0)
                {
                    //ActivatedAt is older or null
                    ActivatedAt = newActivatedAt;
                }
            }

            if (newCompletedAt != null)
            {
                if (newCompletedAt.Value.Kind != DateTimeKind.Utc)
                {
                    newCompletedAt = newCompletedAt.Value.ToUniversalTime();
                }

                if (CompletedAt == null ||
                   DateTime.Compare(CompletedAt.Value, newCompletedAt.Value) < 0)
                {
                    //CompletedAt is older or null
                    CompletedAt = newCompletedAt;
                }
            }

            if (newArchivedAt != null)
            {
                if (newArchivedAt.Value.Kind != DateTimeKind.Utc)
                {
                    newArchivedAt = newArchivedAt.Value.ToUniversalTime();
                }

                if (ArchivedAt == null ||
                   DateTime.Compare(ArchivedAt.Value, newArchivedAt.Value) < 0)
                {
                    //ArchivedAt is older or null
                    ArchivedAt = newArchivedAt;
                }
            }
        }
    }
}