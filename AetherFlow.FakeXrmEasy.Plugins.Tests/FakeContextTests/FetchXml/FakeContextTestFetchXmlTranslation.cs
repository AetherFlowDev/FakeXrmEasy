﻿using Crm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests.FetchXml
{
    /// <summary>
    /// This will test that a fetchxml is correctly translated into a QueryExpression
    /// which was already tested
    ///
    /// We'll leave FetchXml aggregations for a later version
    /// </summary>
    public class FakeContextTestFetchXmlTranslation
    {
        [Test]
        public void When_translating_a_fetch_xml_expression_fetchxml_must_be_an_xml()
        {
            var ctx = new XrmFakedContext();

            Assert.Throws<Exception>(() => XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, "this is not an xml"));
        }

        [Test]
        public void When_translating_a_fetch_xml_expression_first_node_must_be_a_fetch_element_otherwise_exception_is_thrown()
        {
            var ctx = new XrmFakedContext();

            Assert.DoesNotThrow(() => XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, "<fetch><entity name='contact'></entity></fetch>"));
            Assert.Throws<Exception>(() => XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, "<attribute></attribute>"));
            Assert.Throws<Exception>(() => XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, "<entity></entity>"));
        }

        [Test]
        public void When_translating_a_fetch_xml_entity_node_must_have_a_name_attribute()
        {
            var ctx = new XrmFakedContext();

            Assert.Throws<Exception>(() => XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, "<fetch><entity></entity></fetch>"));
            Assert.DoesNotThrow(() => XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, "<fetch><entity name='contact'></entity></fetch>"));
        }

        [Test]
        public void When_translating_a_fetch_xml_attribute_node_must_have_a_name_attribute()
        {
            var ctx = new XrmFakedContext();

            Assert.Throws<Exception>(() => XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, "<fetch><entity name='contact'><attribute></attribute></entity></fetch>"));
            Assert.DoesNotThrow(() => XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, "<fetch><entity name='contact'><attribute name='firstname'></attribute></entity></fetch>"));
        }

        [Test]
        public void When_translating_a_fetch_xml_order_node_must_have_attribute()
        {
            // For (non-aggregate) fetchxml,
            // the order tag must have an attribute specified,
            // and may have the 'descending' attribute specified.
            var ctx = new XrmFakedContext();

            Assert.Throws<Exception>(() => XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, "<fetch><entity name='contact'><order></order></entity></fetch>"));
            Assert.Throws<Exception>(() => XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, "<fetch><entity name='contact'><order descending=''></order></entity></fetch>"));
            Assert.DoesNotThrow(() => XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, "<fetch><entity name='contact'><order attribute='firstname' descending='true'></order></entity></fetch>"));
        }

        [Test]
        public void When_translating_a_fetch_xml_unknown_elements_throw_an_exception()
        {
            var ctx = new XrmFakedContext();
            Assert.Throws<Exception>(() => XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, "<thisdoesntexist></thisdoesntexist>"));
        }

        [Test]
        public void When_translating_a_fetch_xml_expression_queryexpression_name_matches_entity_node()
        {
            var ctx = new XrmFakedContext();
            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='contact'>
                              </entity>
                            </fetch>";

            var query = XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, fetchXml);

            Assert.True(query.EntityName.Equals("contact"));
        }

        [Test]
        public void When_translating_a_fetch_xml_expression_attributes_are_translated_to_a_list_of_columns()
        {
            var ctx = new XrmFakedContext();
            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='contact'>
                                    <attribute name='fullname' />
                                    <attribute name='telephone1' />
                                    <attribute name='contactid' />
                              </entity>
                            </fetch>";

            var query = XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, fetchXml);

            Assert.True(query.ColumnSet != null);
            Assert.AreEqual(false, query.ColumnSet.AllColumns);
            Assert.AreEqual(3, query.ColumnSet.Columns.Count);
            Assert.True(query.ColumnSet.Columns.Contains("fullname"));
            Assert.True(query.ColumnSet.Columns.Contains("telephone1"));
            Assert.True(query.ColumnSet.Columns.Contains("contactid"));
        }

        [Test]
        public void When_translating_a_fetch_xml_expression_all_attributes_is_translated_to_a_columnset_with_all_columns()
        {
            var ctx = new XrmFakedContext();
            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='contact'>
                                    <all-attributes />
                              </entity>
                            </fetch>";

            var query = XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, fetchXml);

            Assert.True(query.ColumnSet != null);
            Assert.AreEqual(true, query.ColumnSet.AllColumns);
        }

        [Test]
        public void When_translating_a_fetch_xml_expression_orderby_ascending_is_correct()
        {
            var ctx = new XrmFakedContext();
            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='contact'>
                                    <attribute name='fullname' />
                                    <attribute name='telephone1' />
                                    <attribute name='contactid' />
                                    <order attribute='fullname' descending='false' />
                              </entity>
                            </fetch>";

            var query = XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, fetchXml);

            Assert.True(query.Orders != null);
            Assert.AreEqual(1, query.Orders.Count);
            Assert.AreEqual("fullname", query.Orders[0].AttributeName);
            Assert.AreEqual(Microsoft.Xrm.Sdk.Query.OrderType.Ascending, query.Orders[0].OrderType);
        }

        [Test]
        public void When_translating_a_fetch_xml_expression_orderby_descending_is_correct()
        {
            var ctx = new XrmFakedContext();
            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='contact'>
                                    <attribute name='fullname' />
                                    <attribute name='telephone1' />
                                    <attribute name='contactid' />
                                    <order attribute='fullname' descending='true' />
                              </entity>
                            </fetch>";

            var query = XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, fetchXml);

            Assert.True(query.Orders != null);
            Assert.AreEqual(1, query.Orders.Count);
            Assert.AreEqual("fullname", query.Orders[0].AttributeName);
            Assert.AreEqual(Microsoft.Xrm.Sdk.Query.OrderType.Descending, query.Orders[0].OrderType);
        }

        [Test]
        public void When_translating_a_fetch_xml_expression_2_orderby_elements_are_translated_correctly()
        {
            var ctx = new XrmFakedContext();
            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='contact'>
                                    <attribute name='fullname' />
                                    <attribute name='telephone1' />
                                    <attribute name='contactid' />
                                    <order attribute='fullname' descending='true' />
                                    <order attribute = 'telephone1' descending = 'false' />
                                  </entity>
                            </fetch>";

            var query = XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, fetchXml);

            Assert.True(query.Orders != null);
            Assert.AreEqual(2, query.Orders.Count);
            Assert.AreEqual("fullname", query.Orders[0].AttributeName);
            Assert.AreEqual(OrderType.Descending, query.Orders[0].OrderType);
            Assert.AreEqual("telephone1", query.Orders[1].AttributeName);
            Assert.AreEqual(OrderType.Ascending, query.Orders[1].OrderType);
        }

        [Test]
        public void When_translating_a_fetch_xml_filter_default_operator_is_and()
        {
            var ctx = new XrmFakedContext();
            ctx.ProxyTypesAssembly = Assembly.GetAssembly(typeof(Contact));

            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='contact'>
                                    <attribute name='fullname' />
                                    <attribute name='telephone1' />
                                    <attribute name='contactid' />
                                        <filter>
                                            <condition attribute='fullname' operator='not-like' value='%Messi' />
                                        </filter>
                                  </entity>
                            </fetch>";

            var query = XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, fetchXml);

            Assert.True(query.Criteria != null);
            Assert.AreEqual(LogicalOperator.And, query.Criteria.FilterOperator);
        }

        [Test]
        public void When_translating_a_fetch_xml_filter_with_and_is_correct()
        {
            var ctx = new XrmFakedContext();
            ctx.ProxyTypesAssembly = Assembly.GetAssembly(typeof(Contact));

            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='contact'>
                                    <attribute name='fullname' />
                                    <attribute name='telephone1' />
                                    <attribute name='contactid' />
                                        <filter type='and'>
                                            <condition attribute='fullname' operator='not-like' value='%Messi' />
                                        </filter>
                                  </entity>
                            </fetch>";

            var query = XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, fetchXml);

            Assert.True(query.Criteria != null);
            Assert.AreEqual(LogicalOperator.And, query.Criteria.FilterOperator);
        }

        [Test]
        public void When_translating_a_fetch_xml_filter_with_or_is_correct()
        {
            var ctx = new XrmFakedContext();
            ctx.ProxyTypesAssembly = Assembly.GetAssembly(typeof(Contact));

            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='contact'>
                                    <attribute name='fullname' />
                                    <attribute name='telephone1' />
                                    <attribute name='contactid' />
                                        <filter type='or'>
                                            <condition attribute='fullname' operator='not-like' value='%Messi' />
                                        </filter>
                                  </entity>
                            </fetch>";

            var query = XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, fetchXml);

            Assert.True(query.Criteria != null);
            Assert.AreEqual(LogicalOperator.Or, query.Criteria.FilterOperator);
        }

        [Test]
        public void When_translating_a_fetch_xml_expression_nested_filters_are_correct()
        {
            var ctx = new XrmFakedContext();
            ctx.ProxyTypesAssembly = Assembly.GetAssembly(typeof(Contact));

            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='contact'>
                                    <attribute name='fullname' />
                                    <attribute name='telephone1' />
                                    <attribute name='contactid' />
                                        <filter type='and'>
                                            <condition attribute='fullname' operator='not-like' value='%Messi' />
                                                <filter type='or'>
                                                    <condition attribute='telephone1' operator='eq' value='123' />
                                                    <condition attribute='telephone1' operator='eq' value='234' />
                                                </filter>
                                        </filter>
                                  </entity>
                            </fetch>";

            var query = XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, fetchXml);

            Assert.True(query.Criteria != null);
            Assert.AreEqual(1, query.Criteria.Conditions.Count);
            Assert.AreEqual(1, query.Criteria.Filters.Count);
            Assert.AreEqual(LogicalOperator.Or, query.Criteria.Filters[0].FilterOperator);
            Assert.AreEqual(2, query.Criteria.Filters[0].Conditions.Count);
        }

        [Test]
        public void When_translating_a_linked_entity_right_result_is_returned()
        {
            var ctx = new XrmFakedContext();
            var fetchXml = @"
                    <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                      <entity name='account'>
                        <attribute name='name' />
                        <attribute name='primarycontactid' />
                        <attribute name='telephone1' />
                        <attribute name='accountid' />
                        <order attribute='name' descending='false' />
                        <link-entity name='account' from='parentaccountid' to='accountid' alias='ab'>
                          <filter type='and'>
                            <condition attribute='name' operator='eq' value='MS' />
                          </filter>
                        </link-entity>
                      </entity>
                    </fetch>
                    ";

            var query = XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, fetchXml);

            Assert.True(query.LinkEntities != null);
            Assert.AreEqual(1, query.LinkEntities.Count);
            Assert.AreEqual("account", query.LinkEntities[0].LinkFromEntityName);
            Assert.AreEqual("accountid", query.LinkEntities[0].LinkFromAttributeName);
            Assert.AreEqual("account", query.LinkEntities[0].LinkToEntityName);
            Assert.AreEqual("parentaccountid", query.LinkEntities[0].LinkToAttributeName);
            Assert.AreEqual("ab", query.LinkEntities[0].EntityAlias);
            Assert.True(query.LinkEntities[0].LinkCriteria != null);
        }

        [Test]
        public void When_translating_a_linked_entity_with_columnset_right_result_is_returned()
        {
            var ctx = new XrmFakedContext();
            var fetchXml = @"
                    <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                      <entity name='account'>
                        <attribute name='name' />
                        <attribute name='primarycontactid' />
                        <attribute name='telephone1' />
                        <attribute name='accountid' />
                        <order attribute='name' descending='false' />
                        <link-entity name='account' from='parentaccountid' to='accountid' alias='ab'>
                          <attribute name='telephone2' />
                        </link-entity>
                      </entity>
                    </fetch>
                    ";

            var query = XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, fetchXml);

            Assert.True(query.LinkEntities != null);
            Assert.AreEqual(1, query.LinkEntities.Count);
            Assert.False(query.LinkEntities[0].Columns.AllColumns);
            Assert.AreEqual(1, query.LinkEntities[0].Columns.Columns.Count);
            Assert.AreEqual("telephone2", query.LinkEntities[0].Columns.Columns[0]);
        }

        [Test]
        public void When_translating_a_linked_entity_with_columnset_with_all_attributes_right_result_is_returned()
        {
            var ctx = new XrmFakedContext();
            var fetchXml = @"
                    <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                      <entity name='account'>
                        <attribute name='name' />
                        <attribute name='primarycontactid' />
                        <attribute name='telephone1' />
                        <attribute name='accountid' />
                        <order attribute='name' descending='false' />
                        <link-entity name='account' from='parentaccountid' to='accountid' alias='ab'>
                          <all-attributes />
                        </link-entity>
                      </entity>
                    </fetch>
                    ";

            var query = XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, fetchXml);

            Assert.True(query.LinkEntities != null);
            Assert.AreEqual(1, query.LinkEntities.Count);
            Assert.True(query.LinkEntities[0].Columns.AllColumns);
        }

        [Test]
        public void When_translating_a_linked_entity_with_filters_right_result_is_returned()
        {
            var ctx = new XrmFakedContext();
            ctx.ProxyTypesAssembly = Assembly.GetAssembly(typeof(Contact));

            var fetchXml = @"
                    <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                      <entity name='account'>
                        <attribute name='name' />
                        <attribute name='primarycontactid' />
                        <attribute name='telephone1' />
                        <attribute name='accountid' />
                        <order attribute='name' descending='false' />
                        <link-entity name='account' from='parentaccountid' to='accountid' alias='ab'>
                          <all-attributes />
                          <filter type='and'>
                                <condition attribute='name' operator='not-like' value='%Messi' />
                                    <filter type='or'>
                                        <condition attribute='telephone1' operator='eq' value='123' />
                                        <condition attribute='telephone1' operator='eq' value='234' />
                                    </filter>
                            </filter>
                        </link-entity>
                      </entity>
                    </fetch>
                    ";

            var query = XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, fetchXml);

            Assert.True(query.LinkEntities != null);
            Assert.AreEqual(1, query.LinkEntities.Count);
            Assert.True(query.LinkEntities[0].LinkCriteria != null);
            Assert.AreEqual(1, query.LinkEntities[0].LinkCriteria.Filters.Count);
            Assert.AreEqual(1, query.LinkEntities[0].LinkCriteria.Conditions.Count);
            Assert.AreEqual(2, query.LinkEntities[0].LinkCriteria.Filters[0].Conditions.Count);
        }

        [Test]
        public void When_executing_fetchxml_right_result_is_returned()
        {
            //This will test a query expression is generated and executed

            var ctx = new XrmFakedContext();
            ctx.Initialize(new List<Entity>()
            {
                new Contact() {Id = Guid.NewGuid(), FirstName = "Leo Messi", Telephone1 = "123" }, //should be returned
                new Contact() {Id = Guid.NewGuid(), FirstName = "Leo Messi", Telephone1 = "234" }, //should be returned
                new Contact() {Id = Guid.NewGuid(), FirstName = "Leo", Telephone1 = "789" }, //shouldnt
                new Contact() {Id = Guid.NewGuid(), FirstName = "Andrés", Telephone1 = "123" }, //shouldnt
            });

            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='contact'>
                                    <attribute name='fullname' />
                                    <attribute name='telephone1' />
                                    <attribute name='contactid' />
                                        <filter type='and'>
                                            <condition attribute='firstname' operator='like' value='%Leo%' />
                                                <filter type='or'>
                                                    <condition attribute='telephone1' operator='eq' value='123' />
                                                    <condition attribute='telephone1' operator='eq' value='234' />
                                                </filter>
                                        </filter>
                                  </entity>
                            </fetch>";

            var retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(fetchXml)
            };

            var service = ctx.GetOrganizationService();
            var response = service.Execute(retrieveMultiple) as RetrieveMultipleResponse;

            Assert.AreEqual(2, response.EntityCollection.Entities.Count);

            //Executing the same via ExecuteMultiple returns also the same
            var response2 = service.RetrieveMultiple(retrieveMultiple.Query);
            Assert.AreEqual(2, response2.Entities.Count);
        }

        [Test]
        public void When_executing_fetchxml_with_count_attribute_only_that_number_of_results_is_returned()
        {
            //This will test a query expression is generated and executed

            var ctx = new XrmFakedContext();

            //Arrange
            var contactList = new List<Entity>();
            for (var i = 0; i < 20; i++)
            {
                contactList.Add(new Contact() { Id = Guid.NewGuid() });
            }
            ctx.Initialize(contactList);

            //Act
            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' count='7'>
                              <entity name='contact'>
                                    <attribute name='fullname' />
                                    <attribute name='telephone1' />
                                    <attribute name='contactid' />
                                  </entity>
                            </fetch>";

            var retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(fetchXml)
            };

            var service = ctx.GetOrganizationService();
            var response = service.Execute(retrieveMultiple) as RetrieveMultipleResponse;

            Assert.AreEqual(7, response.EntityCollection.Entities.Count);
        }

        [Test]
        public void When_executing_fetchxml_with_a_not_integer_count_attribute_exception_is_thrown()
        {
            //This will test a query expression is generated and executed

            var ctx = new XrmFakedContext();

            //Arrange
            //Act
            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' count='asd'>
                              <entity name='contact'>
                                    <attribute name='fullname' />
                                    <attribute name='telephone1' />
                                    <attribute name='contactid' />
                                  </entity>
                            </fetch>";

            var retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(fetchXml)
            };

            var service = ctx.GetOrganizationService();
            Assert.Throws<Exception>(() => service.Execute(retrieveMultiple));
        }

        [Test]
        public void When_filtering_by_a_guid_attribute_right_result_is_returned()
        {
            var context = new XrmFakedContext();
            var accountId = Guid.NewGuid();
            var license1 = new Entity("pl_license");
            license1.Id = Guid.NewGuid();
            license1.Attributes["pl_no"] = 1;
            license1.Attributes["pl_accountid"] = new EntityReference("account", accountId);

            var license2 = new Entity("pl_license");
            license2.Id = Guid.NewGuid();
            license2.Attributes["pl_no"] = 2;
            license2.Attributes["pl_accountid"] = new EntityReference("account", accountId);

            context.Initialize(new List<Entity> { license1, license2 });

            var fetchXml =
                 "<fetch>" +
                 "  <entity name='pl_license'>" +
                 "     <attribute name='pl_no'/>" +
                 "     <filter type='and'>" +
                 "         <condition attribute='pl_accountid' operator='eq' value='{0}' />" +
                 "     </filter>" +
                 "  </entity>" +
                 "</fetch>";
            fetchXml = string.Format(fetchXml, accountId);
            var rows = context.GetOrganizationService().RetrieveMultiple(new FetchExpression(fetchXml));
            Assert.AreEqual(rows.Entities.Count, 2);
        }

        [Test]
        public void When_filtering_by_a_guid_attribute_and_using_proxy_types_right_result_is_returned()
        {
            var context = new XrmFakedContext();
            context.ProxyTypesAssembly = Assembly.GetAssembly(typeof(Account));

            var contactId1 = Guid.NewGuid();
            var contactId2 = Guid.NewGuid();

            var account1 = new Account();
            account1.Id = Guid.NewGuid();
            account1.PrimaryContactId = new EntityReference(Contact.EntityLogicalName, contactId1);

            var account2 = new Account();
            account2.Id = Guid.NewGuid();
            account2.PrimaryContactId = new EntityReference(Contact.EntityLogicalName, contactId2);

            context.Initialize(new List<Entity> { account1, account2 });

            var fetchXml =
                 "<fetch>" +
                 "  <entity name='account'>" +
                 "     <attribute name='name'/>" +
                 "     <filter type='and'>" +
                 "         <condition attribute='primarycontactid' operator='eq' value='{0}' />" +
                 "     </filter>" +
                 "  </entity>" +
                 "</fetch>";
            fetchXml = string.Format(fetchXml, contactId1);
            var rows = context.GetOrganizationService().RetrieveMultiple(new FetchExpression(fetchXml));
            Assert.AreEqual(rows.Entities.Count, 1);
        }

        [Test]
        public void When_filtering_by_an_optionsetvalue_attribute_and_using_proxy_types_right_result_is_returned()
        {
            var context = new XrmFakedContext();
            context.ProxyTypesAssembly = Assembly.GetAssembly(typeof(Account));

            var contactId1 = Guid.NewGuid();
            var contactId2 = Guid.NewGuid();

            var account1 = new Account();
            account1.Id = Guid.NewGuid();
            account1.IndustryCode = new OptionSetValue(2);

            var account2 = new Account();
            account2.Id = Guid.NewGuid();
            account2.IndustryCode = new OptionSetValue(3);

            context.Initialize(new List<Entity> { account1, account2 });

            var fetchXml =
                 "<fetch>" +
                 "  <entity name='account'>" +
                 "     <attribute name='name'/>" +
                 "     <filter type='and'>" +
                 "         <condition attribute='industrycode' operator='eq' value='3' />" +
                 "     </filter>" +
                 "  </entity>" +
                 "</fetch>";
            var rows = context.GetOrganizationService().RetrieveMultiple(new FetchExpression(fetchXml));
            Assert.AreEqual(rows.Entities.Count, 1);
        }

        [Test]
        public void When_filtering_by_a_boolean_attribute_right_result_is_returned()
        {
            var context = new XrmFakedContext();
            context.ProxyTypesAssembly = Assembly.GetAssembly(typeof(Account));

            var account1 = new Entity("account");
            account1.Id = Guid.NewGuid();
            account1["name"] = "Test 1";
            account1["donotemail"] = false;

            var account2 = new Entity("account");
            account2.Id = Guid.NewGuid();
            account2["name"] = "Test 2";
            account2["donotemail"] = true;

            context.Initialize(new List<Entity> { account1, account2 });

            var fetchXml =
                 "<fetch>" +
                 "  <entity name='account'>" +
                 "     <attribute name='name'/>" +
                 "     <filter type='and'>" +
                 "         <condition attribute='donotemail' operator='eq' value='0' />" +
                 "     </filter>" +
                 "  </entity>" +
                 "</fetch>";
            var rows = context.GetOrganizationService().RetrieveMultiple(new FetchExpression(fetchXml));
            Assert.AreEqual(rows.Entities.Count, 1);
            Assert.AreEqual(rows.Entities[0]["name"], account1["name"]);
        }

        [Test]
        public void When_filtering_by_a_boolean_attribute_and_using_proxy_types_right_result_is_returned()
        {
            var context = new XrmFakedContext();
            context.ProxyTypesAssembly = Assembly.GetAssembly(typeof(Account));

            var account1 = new Account();
            account1.Id = Guid.NewGuid();
            account1.Name = "Test 1";
            account1.DoNotEMail = false;

            var account2 = new Account();
            account2.Id = Guid.NewGuid();
            account2.Name = "Test 2";
            account2.DoNotEMail = true;

            context.Initialize(new List<Entity> { account1, account2 });

            var fetchXml =
                 "<fetch>" +
                 "  <entity name='account'>" +
                 "     <attribute name='name'/>" +
                 "     <filter type='and'>" +
                 "         <condition attribute='donotemail' operator='eq' value='0' />" +
                 "     </filter>" +
                 "  </entity>" +
                 "</fetch>";
            var rows = context.GetOrganizationService().RetrieveMultiple(new FetchExpression(fetchXml));
            Assert.AreEqual(rows.Entities.Count, 1);
            Assert.AreEqual(rows.Entities[0].ToEntity<Account>().Name, account1.Name);
        }

        [Test]
        public void When_filtering_by_an_enum_attribute_and_using_proxy_types_right_result_is_returned()
        {
            var fakedContext = new XrmFakedContext { };

            var entityAccount = new Account { Id = Guid.NewGuid(), Name = "Test Account", LogicalName = "account" };
            var entityContact = new Contact { Id = Guid.NewGuid(), ParentCustomerId = entityAccount.ToEntityReference(), EMailAddress1 = "test@sample.com" };

            var entityCase = new Incident
            {
                Id = Guid.NewGuid(),
                PrimaryContactId = entityContact.ToEntityReference(),
                CustomerId = entityAccount.ToEntityReference(),
                Title = "Unit Test Case"
            };

            entityCase["statecode"] = new OptionSetValue((int)IncidentState.Active);

            fakedContext.Initialize(new List<Entity>() {
               entityAccount,entityContact, entityCase
            });

            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' >
              <entity name='incident' >
                <attribute name='incidentid' />
                <attribute name='statecode' />
                <order attribute='createdon' descending='true' />
                 <filter type='and' >
                  <condition attribute='statecode' operator='neq' value='2' />
                </filter>
              </entity>
            </fetch>";

            var rows = fakedContext.GetOrganizationService().RetrieveMultiple(new FetchExpression(fetchXml));
            Assert.AreEqual(rows.Entities.Count, 1);
        }

        [Test]
        public void When_filtering_by_a_money_attribute_and_using_proxy_types_right_result_is_returned()
        {
            var context = new XrmFakedContext();
            context.ProxyTypesAssembly = Assembly.GetAssembly(typeof(Account));

            var contactId1 = Guid.NewGuid();
            var contactId2 = Guid.NewGuid();

            var account1 = new Account();
            account1.Id = Guid.NewGuid();
            account1.MarketCap = new Money(123.45m);

            var account2 = new Account();
            account2.Id = Guid.NewGuid();
            account2.MarketCap = new Money(223.45m);

            context.Initialize(new List<Entity> { account1, account2 });

            var fetchXml =
                 "<fetch>" +
                 "  <entity name='account'>" +
                 "     <attribute name='name'/>" +
                 "     <filter type='and'>" +
                 "         <condition attribute='marketcap' operator='eq' value='123.45' />" +
                 "     </filter>" +
                 "  </entity>" +
                 "</fetch>";
            var rows = context.GetOrganizationService().RetrieveMultiple(new FetchExpression(fetchXml));
            Assert.AreEqual(rows.Entities.Count, 1);
        }

        [Test]
        public void When_querying_the_same_entity_records_with_joins_no_collection_modified_exception_is_thrown()
        {
            var fakedContext = new XrmFakedContext { };
            var service = fakedContext.GetOrganizationService();

            var entityAccount = new Account { Id = Guid.NewGuid(), Name = "My First Faked Account yeah!", LogicalName = "account" };
            var entityContact = new Contact { Id = Guid.NewGuid(), ParentCustomerId = entityAccount.ToEntityReference() };

            var entityBusinessUnit = new BusinessUnit { Name = "TestBU", BusinessUnitId = Guid.NewGuid() };

            var initiatingUser = new SystemUser
            {
                Id = Guid.NewGuid(),
                FirstName = "TestUser",
                DomainName = "TestDomain",
                BusinessUnitId = entityBusinessUnit.ToEntityReference()
            };

            fakedContext.Initialize(new List<Entity>() {
               entityBusinessUnit,entityAccount,entityContact,initiatingUser
            });

            var fetchXml = @"
                    <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='systemuser'>
                        <attribute name='fullname' />
                         <attribute name='systemuserid' />
                         <attribute name='businessunitid' />
                         <filter type='and'>
                          <condition attribute='systemuserid' operator='eq' uitype='systemuser' value='#userId#' />
                         </filter>
                            <link-entity name='businessunit' from='businessunitid' to='businessunitid' alias='bu' intersect='true' >
                                <attribute name='name' />
                            </link-entity>
                      </entity>
                    </fetch>
                ";

            var UserRequest = new RetrieveMultipleRequest { Query = new FetchExpression(fetchXml.Replace("#userId#", initiatingUser.Id.ToString())) };
            var response = ((RetrieveMultipleResponse)service.Execute(UserRequest));

            var entities = response.EntityCollection.Entities;
            Assert.True(entities.Count == 1);
            Assert.True(entities[0].Attributes.ContainsKey("bu.name"));
            Assert.IsInstanceOf<AliasedValue>(entities[0]["bu.name"]);
            Assert.AreEqual("TestBU", (entities[0]["bu.name"] as AliasedValue).Value.ToString());
        }

        [Test]
        public void When_querying_fetchxml_with_linked_entities_linked_entity_properties_match_the_equivalent_linq_expression()
        {
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            var contact = new Contact()
            {
                Id = Guid.NewGuid(),
                FirstName = "Lionel"
            };

            var account = new Account()
            {
                Id = Guid.NewGuid(),
                PrimaryContactId = contact.ToEntityReference()
            };

            context.Initialize(new List<Entity>
            {
                contact, account
            });

            var fetchXml = @"
                    <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='account'>
                        <attribute name='name' />
                        <attribute name='primarycontactid' />
                        <attribute name='telephone1' />
                        <attribute name='accountid' />
                        <order attribute='name' descending='false' />
                        <link-entity name='contact' from='contactid' to='primarycontactid' alias='aa'>
                          <attribute name='firstname' />
                          <filter type='and'>
                            <condition attribute='firstname' operator='eq' value='Lionel' />
                          </filter>
                        </link-entity>
                      </entity>
                    </fetch>
                ";

            //Equivalent linq query
            using (var ctx = new XrmServiceContext(service))
            {
                var linqQuery = (from a in ctx.CreateQuery<Account>()
                                 join c in ctx.CreateQuery<Contact>() on a.PrimaryContactId.Id equals c.ContactId
                                 where c.FirstName == "Lionel"
                                 select new
                                 {
                                     Account = a,
                                     Contact = c
                                 }).ToList();
            }

            var queryExpression = XrmFakedContext.TranslateFetchXmlToQueryExpression(context, fetchXml);
            Assert.True(queryExpression.LinkEntities.Count == 1);

            var linkedEntity = queryExpression.LinkEntities[0];
            Assert.AreEqual(linkedEntity.LinkFromAttributeName, "primarycontactid");
            Assert.AreEqual(linkedEntity.LinkToAttributeName, "contactid");
            Assert.AreEqual(linkedEntity.JoinOperator, JoinOperator.Inner);

            var request = new RetrieveMultipleRequest { Query = new FetchExpression(fetchXml) };
            var response = ((RetrieveMultipleResponse)service.Execute(request));

            var entities = response.EntityCollection.Entities;
            Assert.True(entities.Count == 1);
            Assert.True(entities[0].Attributes.ContainsKey("aa.firstname"));
            Assert.IsInstanceOf<AliasedValue>(entities[0]["aa.firstname"]);
            Assert.AreEqual("Lionel", (entities[0]["aa.firstname"] as AliasedValue).Value.ToString());
        }

        [Test]
        public void When_querying_fetchxml_with_linked_entities_with_left_outer_join_right_result_is_returned()
        {
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            var contact = new Contact()
            {
                Id = Guid.NewGuid(),
                FirstName = "Lionel"
            };

            var account = new Account() { Id = Guid.NewGuid(), PrimaryContactId = contact.ToEntityReference() };
            var account2 = new Account() { Id = Guid.NewGuid(), PrimaryContactId = null };

            context.Initialize(new List<Entity>
            {
                contact, account, account2
            });

            var fetchXml = @"
                    <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='account'>
                        <attribute name='name' />
                        <attribute name='primarycontactid' />
                        <attribute name='telephone1' />
                        <attribute name='accountid' />
                        <order attribute='name' descending='false' />
                        <link-entity name='contact' from='contactid' to='primarycontactid' alias='aa' link-type='outer'>
                          <attribute name='firstname' />
                        </link-entity>
                        <filter type='and'>
                            <condition attribute='statecode' operator='eq' value='0' />
                        </filter>
                      </entity>
                    </fetch>
                ";

            //Translated correctly
            var queryExpression = XrmFakedContext.TranslateFetchXmlToQueryExpression(context, fetchXml);
            Assert.True(queryExpression.LinkEntities.Count == 1);

            var linkedEntity = queryExpression.LinkEntities[0];
            Assert.AreEqual(linkedEntity.JoinOperator, JoinOperator.LeftOuter);

            //Executed correctly
            var request = new RetrieveMultipleRequest { Query = new FetchExpression(fetchXml) };
            var response = ((RetrieveMultipleResponse)service.Execute(request));

            var entities = response.EntityCollection.Entities;
            Assert.True(entities.Count == 2);
            Assert.AreEqual("Lionel", (entities[0]["aa.firstname"] as AliasedValue).Value.ToString());
            Assert.False(entities[1].Attributes.ContainsKey("aa.firstname"));
        }

        [Test]
        public void When_translating_a_fetch_xml_return_total_count_is_translated_correctly()
        {
            var ctx = new XrmFakedContext();
            ctx.ProxyTypesAssembly = Assembly.GetAssembly(typeof(Contact));

            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' returntotalrecordcount='true'>
                              <entity name='contact'>
                                    <attribute name='fullname' />
                                    <attribute name='telephone1' />
                                    <attribute name='contactid' />
                              </entity>
                            </fetch>";

            var query = XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, fetchXml);

            Assert.True(query.PageInfo.ReturnTotalRecordCount);
        }

        [Test]
        public void When_translating_a_fetch_xml_distinct_is_translated_correctly()
        {
            var ctx = new XrmFakedContext();
            ctx.ProxyTypesAssembly = Assembly.GetAssembly(typeof(Contact));

            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' returntotalrecordcount='true'>
                              <entity name='contact'>
                                    <attribute name='fullname' />
                                    <attribute name='telephone1' />
                                    <attribute name='contactid' />
                              </entity>
                            </fetch>";

            var query = XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, fetchXml);

            Assert.True(query.Distinct);
        }
    }
}