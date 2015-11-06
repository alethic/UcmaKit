using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.Rtc.Collaboration;

namespace UcmaKit.Rtc.Util
{

    public static class ConversationContextChannelExtensions
    {

        public static Task EstablishAsync(this ConversationContextChannel self, Guid applicationId, ConversationContextChannelEstablishOptions options)
        {
            return Task.Factory.FromAsync<Guid, ConversationContextChannelEstablishOptions>(
                self.BeginEstablish,
                self.EndEstablish,
                applicationId,
                options,
                null);
        }

        public static Task SendDataAsync(this ConversationContextChannel self, ContentType contentType, byte[] contentBody)
        {
            return Task.Factory.FromAsync<ContentType, byte[]>(
                self.BeginSendData,
                self.EndSendData,
                contentType,
                contentBody,
                null);
        }

    }

}
