﻿// <copyright>
// Copyright Southeast Christian Church
//
// Licensed under the  Southeast Christian Church License (the "License");
// you may not use this file except in compliance with the License.
// A copy of the License shoud be included with this file.
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using org.secc.FamilyCheckin.Model;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace org.secc.FamilyCheckin.Cache
{
    [Serializable]
    [DataContract]
    public class MobileCheckinRecordCache : ModelCache<MobileCheckinRecordCache, MobileCheckinRecord>
    {
        [DataMember]
        public string AccessKey { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public int FamilyGroupId { get; set; }

        [DataMember]
        public string SerializedCheckInState { get; set; }

        [DataMember]
        public DateTime? ExpirationDateTime { get; set; }

        [DataMember]
        public List<int> AttendanceIds { get; set; }

        public List<Attendance> GetAttendances( RockContext rockContext )
        {
            AttendanceService attendanceService = new AttendanceService( rockContext );
            return attendanceService.Queryable().Where( a => AttendanceIds.Contains( a.Id ) ).ToList();
        }

        public static MobileCheckinRecordCache GetByAccessKey( string accessKey )
        {
            return All().Where( r => r.AccessKey == accessKey ).FirstOrDefault();
        }

        public static MobileCheckinRecordCache GetByUserName( string userName )
        {
            return All().Where( r => r.UserName == userName ).FirstOrDefault();
        }

        public static MobileCheckinRecordCache GetByFamilyGroupId( int familyGroupId )
        {
            return All().Where( r => r.FamilyGroupId == familyGroupId ).FirstOrDefault();
        }

        public override void SetFromEntity( IEntity entity )
        {
            base.SetFromEntity( entity );

            var record = entity as MobileCheckinRecord;
            if ( record == null )
                return;

            Id = record.Id;
            Guid = record.Guid;
            AccessKey = record.AccessKey;
            UserName = record.UserName;
            FamilyGroupId = record.FamilyGroupId;
            ExpirationDateTime = record.ExpirationDateTime;
            SerializedCheckInState = record.SerializedCheckInState;
            AttendanceIds = record.Attendances.Select( a => a.Id ).ToList();
        }

        public MobileCheckinRecord GetEntity( RockContext rockContext )
        {
            MobileCheckinRecordService mobileCheckinRecordService = new MobileCheckinRecordService( rockContext );
            return mobileCheckinRecordService.Get( Id );
        }
    }
}
