using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExampleProviderSite.Repositories
{
    public class ContactsRepository
    {
        public List<Contact> GetContactsForUser(string userName)
        {
            switch (userName)
            {
                case "john":
                    return new List<Contact>
                               {
                                   new Contact { FullName="Al Johnson", Email="jake@test.com"},
                                   new Contact{ FullName="Mikey Miles", Email="mmiles@test.com"}
                               };
                case "jane":
                    return new List<Contact>
                               {
                                   new Contact{ FullName="Drake Diego", Email="drake.diego@test.com"},
                                   new Contact {FullName="Lake Winterse",Email="wintersl@test.com"}
                               };
                default:
                    throw new Exception("unknown user");
            }
        }
    }

    public class Contact
    {
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
