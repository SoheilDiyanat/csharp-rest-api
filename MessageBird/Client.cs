﻿using MessageBird.Net.ProxyConfigurationInjector;
using MessageBird.Objects;
using MessageBird.Resources;
using MessageBird.Net;
using MessageBird.Utilities;
using System;
using System.Collections.Generic;
using MessageBird.Objects.VoiceCalls;
using MessageBird.Resources.VoiceCalls;

namespace MessageBird
{

    public partial class Client
    {
        public enum Features
        {
            EnableWhatsAppSandboxConversations = 1
        }

        private readonly IRestClient restClient;
        private readonly Features[] features;

        private bool useConversationsWhatsAppSandbox;

        private Client(IRestClient restClient, Features[] features = null)
        {
            this.restClient = restClient;
            this.features = features;
            if (this.features != null && Array.IndexOf(this.features, Features.EnableWhatsAppSandboxConversations) >= 0)
            {
                this.useConversationsWhatsAppSandbox = true;
            }
        }

        public static Client Create(IRestClient restClient, Features[] features = null)
        {
            ParameterValidator.IsNotNull(restClient, "restClient");

            return new Client(restClient, features);
        }

        public static Client CreateDefault(string accessKey, IProxyConfigurationInjector proxyConfigurationInjector = null, Features[] features = null)
        {
            ParameterValidator.IsNotNullOrWhiteSpace(accessKey, "accessKey");

            return new Client(new RestClient(accessKey, proxyConfigurationInjector), features);
        }

        public Message SendMessage(string originator, string body, long[] msisdns, MessageOptionalArguments optionalArguments = null)
        {
            ParameterValidator.IsNotNullOrWhiteSpace(originator, "originator");
            ParameterValidator.IsNotNullOrWhiteSpace(body, "body");
            ParameterValidator.ContainsAtLeast(msisdns, 1, "msisdns");

            if (optionalArguments != null)
            {
                ParameterValidator.IsValidMessageType(optionalArguments.Type);
            }

            var recipients = new Recipients(msisdns);
            var message = new Message(originator, body, recipients, optionalArguments);

            var messages = new Messages(message);
            var result = restClient.Create(messages);

            return result.Object as Message;
        }

        public Objects.Verify SendVerifyToken(string id, string token)
        {
            ParameterValidator.IsNotNullOrWhiteSpace(id, "id");
            ParameterValidator.IsNotNullOrWhiteSpace(token, "token");

            var verify = new Objects.Verify(id, token);
            var verifyResource = new Resources.Verify(verify);
            var result = restClient.Retrieve(verifyResource);

            return result.Object as Objects.Verify;
        }

        // Alias for the old constructor so that it remains backwards compatible
        public Objects.Verify CreateVerify(string recipient, VerifyOptionalArguments arguments = null)
        {
            ParameterValidator.IsNotNullOrWhiteSpace(recipient, "recipient");

            return CreateVerify(Convert.ToInt64(recipient), arguments);
        }

        public Objects.Verify CreateVerify(long recipient, VerifyOptionalArguments arguments = null)
        {
            var verify = new Objects.Verify(recipient, arguments);
            var verifyResource = new Resources.Verify(verify);
            var result = restClient.Create(verifyResource);

            return result.Object as Objects.Verify;
        }

        public void DeleteVerify(string id)
        {
            ParameterValidator.IsNotNullOrWhiteSpace(id, "id");

            var verify = new Objects.Verify(id);
            var verifyResource = new Resources.Verify(verify);

            restClient.Delete(verifyResource);
        }

        public Objects.Verify ViewVerify(string id)
        {
            ParameterValidator.IsNotNullOrWhiteSpace(id, "id");

            var verify = new Objects.Verify(id);
            var verifyResource = new Resources.Verify(verify);
            var result = restClient.Retrieve(verifyResource);

            return result.Object as Objects.Verify;
        }

        public Message ViewMessage(string id)
        {
            ParameterValidator.IsNotNullOrWhiteSpace(id, "id");

            var messageToView = new Messages(new Message(id));
            var result = restClient.Retrieve(messageToView);

            return result.Object as Message;
        }

        public VoiceMessage SendVoiceMessage(string body, long[] msisdns, VoiceMessageOptionalArguments optionalArguments = null)
        {
            ParameterValidator.IsNotNullOrWhiteSpace(body, "body");
            ParameterValidator.ContainsAtLeast(msisdns, 1, "msisdns");

            var recipients = new Recipients(msisdns);
            var voiceMessage = new VoiceMessage(body, recipients, optionalArguments);
            var voiceMessages = new VoiceMessages(voiceMessage);
            var result = restClient.Create(voiceMessages);

            return result.Object as VoiceMessage;
        }

        public VoiceMessage ViewVoiceMessage(string id)
        {
            ParameterValidator.IsNotNullOrWhiteSpace(id, "id");

            var voiceMessageToView = new VoiceMessages(new VoiceMessage(id));
            var result = restClient.Retrieve(voiceMessageToView);

            return result.Object as VoiceMessage;
        }

        public Objects.Hlr RequestHlr(long msisdn, string reference)
        {
            ParameterValidator.IsNotNullOrWhiteSpace(reference, "reference");

            var hlrToRequest = new Resources.Hlr(new Objects.Hlr(msisdn, reference));
            var result = restClient.Create(hlrToRequest);

            return result.Object as Objects.Hlr;
        }

        public Objects.Hlr ViewHlr(string id)
        {
            ParameterValidator.IsNotNullOrWhiteSpace(id, "id");

            var hlrToView = new Resources.Hlr(new Objects.Hlr(id));
            var result = restClient.Retrieve(hlrToView);

            return result.Object as Objects.Hlr;
        }

        public Objects.Balance Balance()
        {
            var balance = new Resources.Balance();
            var result = restClient.Retrieve(balance);

            return result.Object as Objects.Balance;
        }

        public Objects.Lookup ViewLookup(long phonenumber, LookupOptionalArguments optionalArguments = null)
        {
            var lookup = new Resources.Lookup(new Objects.Lookup(phonenumber, optionalArguments));
            var result = restClient.Retrieve(lookup);

            return result.Object as Objects.Lookup;
        }

        public Objects.LookupHlr RequestLookupHlr(long phonenumber, string reference, LookupHlrOptionalArguments optionalArguments = null)
        {
            ParameterValidator.IsNotNullOrWhiteSpace(reference, "reference");

            var lookupHlr = new Resources.LookupHlr(new Objects.LookupHlr(phonenumber, reference, optionalArguments));
            var result = restClient.Create(lookupHlr);

            return result.Object as Objects.LookupHlr;
        }

        public Objects.LookupHlr ViewLookupHlr(long phonenumber, LookupHlrOptionalArguments optionalArguments = null)
        {
            var lookupHlr = new Resources.LookupHlr(new Objects.LookupHlr(phonenumber, optionalArguments));
            var result = restClient.Retrieve(lookupHlr);

            return result.Object as Objects.LookupHlr;
        }

        public Contact CreateContact(long msisdn, ContactOptionalArguments optionalArguments = null)
        {
            var contact = new Contact { Msisdn = msisdn };
            if (optionalArguments != null)
            {
                contact.FirstName = optionalArguments.FirstName;
                contact.LastName = optionalArguments.LastName;
                contact.CustomDetails = new ContactCustomDetails
                {
                    Custom1 = optionalArguments.Custom1,
                    Custom2 = optionalArguments.Custom2,
                    Custom3 = optionalArguments.Custom3,
                    Custom4 = optionalArguments.Custom4,
                };
            }

            var result = restClient.Create(new Contacts(contact));

            return result.Object as Contact;
        }

        public void DeleteContact(string id)
        {
            restClient.Delete(new Contacts(new Contact { Id = id }));
        }

        public ContactList ListContacts(int limit = 20, int offset = 0)
        {
            var contactLists = new ContactLists();

            var contactList = (ContactList)contactLists.Object;
            contactList.Limit = limit;
            contactList.Offset = offset;

            restClient.Retrieve(contactLists);

            return contactLists.Object as ContactList;
        }

        public Contact ViewContact(string id)
        {
            ParameterValidator.IsNotNullOrWhiteSpace(id, "id");

            var contacts = new Contacts(new Contact { Id = id });
            restClient.Retrieve(contacts);

            return contacts.Object as Contact;
        }

        public Contact UpdateContact(string id, ContactOptionalArguments optionalArguments)
        {
            ParameterValidator.IsNotNullOrWhiteSpace(id, "id");

            var customDetails = new ContactCustomDetails
            {
                Custom1 = optionalArguments.Custom1,
                Custom2 = optionalArguments.Custom2,
                Custom3 = optionalArguments.Custom3,
                Custom4 = optionalArguments.Custom4,
            };

            var contacts = new Contacts(new Contact
            {
                Id = id,
                FirstName = optionalArguments.FirstName,
                LastName = optionalArguments.LastName,
                CustomDetails = customDetails,
            });

            restClient.Update(contacts);

            return contacts.Object as Contact;
        }

        public Group CreateGroup(string name)
        {
            ParameterValidator.IsNotNullOrWhiteSpace(name, "name");

            var groups = new Groups(new Group { Name = name });
            var result = restClient.Create(groups);

            return result.Object as Group;
        }

        public void DeleteGroup(string id)
        {
            ParameterValidator.IsNotNullOrWhiteSpace(id, "id");

            var groups = new Groups(new Group { Id = id });

            restClient.Delete(groups);
        }

        public GroupList ListGroups(int limit = 20, int offset = 0)
        {
            var groupLists = new GroupLists();

            var groupList = (GroupList)groupLists.Object;
            groupList.Limit = limit;
            groupList.Offset = offset;

            restClient.Retrieve(groupLists);

            return groupLists.Object as GroupList;
        }

        public Group UpdateGroup(string id, string name)
        {
            ParameterValidator.IsNotNullOrWhiteSpace(id, "id");

            var groups = new Groups(new Group
            {
                Id = id,
                Name = name,
            });

            restClient.Update(groups);

            return groups.Object as Group;
        }

        public Group ViewGroup(string id)
        {
            ParameterValidator.IsNotNullOrWhiteSpace(id, "id");

            var groups = new Groups(new Group { Id = id });
            restClient.Retrieve(groups);

            return groups.Object as Group;
        }

        public void AddContactsToGroup(string groupId, IEnumerable<string> contactIds)
        {
            ParameterValidator.IsNotNullOrWhiteSpace(groupId, "groupId");

            var uri = string.Format("groups/{0}?{1}", groupId, GetAddContactsToGroupQuery(contactIds));

            restClient.PerformHttpRequest("GET", uri, HttpStatusCode.NoContent, baseUrl: Resource.DefaultBaseUrl);
        }

        /// <summary>
        /// Gets a query string to add contact IDs to a group. This uses a
        /// specific format: ids[]=foo&ids[]=bar. Given the structure of
        /// RestClient, this is easier done by providing the HTTP method as URL
        /// parameter. See:
        /// https://developers.messagebird.com/docs/groups#add-contact-to-group
        /// https://developers.messagebird.com/docs/groups#add-contact-to-group
        /// </summary>
        private string GetAddContactsToGroupQuery(IEnumerable<string> contactIds)
        {
            var parameters = new List<string>();
            parameters.Add("_method=PUT");

            foreach (var contactId in contactIds)
            {
                parameters.Add("ids[]=" + contactId);
            }

            return string.Join("&", parameters);
        }

        public void RemoveContactFromGroup(string groupId, string contactId)
        {
            ParameterValidator.IsNotNullOrWhiteSpace(groupId, "groupId");
            ParameterValidator.IsNotNullOrWhiteSpace(contactId, "contactId");

            var uri = string.Format("groups/{0}/contacts/{1}", groupId, contactId);

            restClient.PerformHttpRequest("DELETE", uri, HttpStatusCode.NoContent, baseUrl: Resource.DefaultBaseUrl);
        }

        /// <summary>
        /// This request retrieves a listing of all call flows.
        /// </summary>
        /// <param name="limit">Set how many records will return from the server</param>
        /// <param name="offset">Identify the starting point to return rows from a result</param>
        /// <returns>If successful, this request will return an object with a data, _links and pagination properties.</returns>
        public VoiceCallFlowList ListVoiceCallFlows(int limit = 20, int offset = 0)
        {
            var resource = new VoiceCallFlowLists();

            var list = (VoiceCallFlowList)resource.Object;
            list.Limit = limit;
            list.Offset = offset;

            restClient.Retrieve(resource);

            return (VoiceCallFlowList)resource.Object;
        }

        /// <summary>
        /// This request retrieves a call flow resource.<para />
        /// The single parameter is the unique ID that was returned upon creation. 
        /// </summary>
        /// <param name="id">The unique ID which was returned upon creation of a call flow.</param>
        /// <returns></returns>
        public VoiceCallFlow ViewVoiceCallFlow(string id)
        {
            ParameterValidator.IsNotNullOrWhiteSpace(id, "id");

            var resource = new VoiceCallFlows(new VoiceCallFlow() { Id = id });
            restClient.Retrieve(resource);

            return (VoiceCallFlow)resource.Object;
        }

        /// <summary>
        /// Creating a call flow
        /// </summary>
        /// <param name="request"></param>
        /// <returns>If successful, this request will return an object with a data property, which is an array that has a single call flow object. If the request failed, an error object will be returned.</returns>
        public VoiceCallFlow CreateVoiceCallFlow(VoiceCallFlow request)
        {
            ParameterValidator.IsNotNullOrWhiteSpace(request.Title, "title");
            ParameterValidator.IsNotNull(request.Steps, "steps");

            var voiceCallFlows = new VoiceCallFlows(new VoiceCallFlow { Title = request.Title, Steps = request.Steps, Record = request.Record });
            var result = restClient.Create(voiceCallFlows);

            return (VoiceCallFlow)result.Object;
        }

        /// <summary>
        /// This request deletes a call flow. The single parameter is the unique ID that was returned upon creation.<br/>
        /// If successful, this request will return an HTTP header of 204 No Content and an empty response. If the request failed, an error object will be returned.
        /// </summary>
        /// <param name="id">The unique ID which was returned upon creation of a call flow.</param>
        public void DeleteVoiceCallFlow(string id)
        {
            ParameterValidator.IsNotNullOrWhiteSpace(id, "id");

            var voiceCallFlows = new VoiceCallFlows(new VoiceCallFlow { Id = id });

            restClient.Delete(voiceCallFlows);
        }

        /// <summary>
        /// This request updates a call flow resource. The single parameter is the unique ID that was returned upon creation.<br/>
        /// If successful, this request will return an object with a data property, which is an array that has a single call flow object. If the request failed, an error object will be returned.
        /// </summary>
        /// <param name="id">The unique ID which was returned upon creation of a call flow.</param>
        /// <param name="voiceCallFlow"></param>
        /// <returns></returns>
        public VoiceCallFlow UpdateVoiceCallFlow(string id, VoiceCallFlow voiceCallFlow)
        {

            ParameterValidator.IsNotNullOrWhiteSpace(voiceCallFlow.Title, "title");
            ParameterValidator.IsNotNull(voiceCallFlow.Steps, "steps");

            var voiceCallFlows = new VoiceCallFlows(new VoiceCallFlow { Id = id, Title = voiceCallFlow.Title, Steps = voiceCallFlow.Steps, Record = voiceCallFlow.Record });
            var result = restClient.Update(voiceCallFlows);
            return (VoiceCallFlow)result.Object;
        }
    }


}

