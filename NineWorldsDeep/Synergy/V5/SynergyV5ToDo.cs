using NineWorldsDeep.Core;
using System;

namespace NineWorldsDeep.Synergy.V5
{
    public class SynergyV5ToDo
    {
        public static string TO_DO_STATUS_INDETERMINATE = "Status Indeterminate";
        public static string TO_DO_STATUS_ARCHIVED = "Archived";
        public static string TO_DO_STATUS_ACTIVATED = "Activated";
        public static string TO_DO_STATUS_COMPLETED = "Completed";

        public int ToDoId { get; set; }
        public DateTime? ActivatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public DateTime? ArchivedAt { get; private set; }
        public string Status { get { return ProcessStatus(); } }
        
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

        internal void Activate()
        {
            ActivatedAt = TimeStamp.NowUTC();
        }

        internal void Complete()
        {
            CompletedAt = TimeStamp.NowUTC();
        }

        internal void Archive()
        {
            ArchivedAt = TimeStamp.NowUTC();
        }

        public bool IsActive()
        {
            return Status.Contains(TO_DO_STATUS_ACTIVATED);
        }

        public bool IsCompleted()
        {
            return Status.Contains(TO_DO_STATUS_COMPLETED);
        }

        public bool IsArchived()
        {
            return Status.Contains(TO_DO_STATUS_ARCHIVED);
        }

        private string ProcessStatus()
        {
            if (ActivatedAt == null && CompletedAt == null && ArchivedAt == null)
            {
                return TO_DO_STATUS_INDETERMINATE;
            }

            if (ActivatedAt == null && CompletedAt == null)
            {
                //only Archived is non-null
                return TO_DO_STATUS_ARCHIVED;
            }

            if(CompletedAt == null && ArchivedAt == null)
            {
                //only Activated
                return TO_DO_STATUS_ACTIVATED;
            }

            if(ActivatedAt == null && ArchivedAt == null)
            {
                //only Completed
                return TO_DO_STATUS_COMPLETED;
            }

            if(ActivatedAt == null)
            {
                //archived and completed 
                return TO_DO_STATUS_ARCHIVED + ", " + TO_DO_STATUS_COMPLETED;
            }

            if(ArchivedAt == null)
            {
                //activated and completed
                if (ActivatedNewerThanCompletedAssumingNonNullInput())
                {
                    return TO_DO_STATUS_ACTIVATED;
                }
                else
                {
                    return TO_DO_STATUS_ACTIVATED + ", " + TO_DO_STATUS_COMPLETED;
                }                
            }

            if(CompletedAt == null)
            {
                //activated and archived
                if(DateTime.Compare(ActivatedAt.Value, ArchivedAt.Value) < 0)
                {
                    //Activated is older
                    return TO_DO_STATUS_ARCHIVED;
                }
                else
                {
                    return TO_DO_STATUS_ACTIVATED;
                }
            }
            
            //if we get here, all three are non-null
            if (DateTime.Compare(ActivatedAt.Value, ArchivedAt.Value) < 0)
            {
                //Activated is older
                return TO_DO_STATUS_ARCHIVED + ", " + TO_DO_STATUS_COMPLETED;
            }
            else
            {
                if (ActivatedNewerThanCompletedAssumingNonNullInput())
                {
                    return TO_DO_STATUS_ACTIVATED;
                }
                else
                {
                    return TO_DO_STATUS_ACTIVATED + ", " + TO_DO_STATUS_COMPLETED;
                };
            }
        }

        private bool ActivatedNewerThanCompletedAssumingNonNullInput()
        {
            if (DateTime.Compare(ActivatedAt.Value, CompletedAt.Value) < 0)
            {
                //ActivatedAt is older
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}