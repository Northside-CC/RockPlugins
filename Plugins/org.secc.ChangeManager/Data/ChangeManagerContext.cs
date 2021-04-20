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
using System.Data.Entity;
using org.secc.ChangeManager.Model;

namespace org.secc.ChangeManager.Data
{
    public class ChangeManagerContext : Rock.Data.DbContext
    {
        public DbSet<ChangeRecord> ChangeRecord { get; set; }
        public DbSet<ChangeRequest> ChangeRequest { get; set; }
        public ChangeManagerContext()
            : base( "RockContext" )
        {

        }

        protected override void OnModelCreating( DbModelBuilder modelBuilder )
        {
            // we don't want this context to create a database or look for EF Migrations, do set the Initializer to null
            Database.SetInitializer<ChangeManagerContext>( new NullDatabaseInitializer<ChangeManagerContext>() );

            Rock.Data.ContextHelper.AddConfigurations( modelBuilder );
            modelBuilder.Configurations.AddFromAssembly( System.Reflection.Assembly.GetExecutingAssembly() );
        }
    }
}
