// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Tests.Common;
using System.Text;
using System.Xml;
using Xunit;

public static class MessageTest
{
    private const string s_action = "http://tempuri.org/someserviceendpoint";

    [Theory]
    [MemberData("MessageVersionsWithEnvelopeAndAddressingVersions", MemberType = typeof(TestData))]
    public static void MessageVersion_Verify_AddressingVersions_And_EnvelopeVersions(MessageVersion messageVersion, EnvelopeVersion envelopeVersion, AddressingVersion addressingVersion)
    {
        Assert.Equal<EnvelopeVersion>(envelopeVersion, messageVersion.Envelope);
        Assert.Equal<AddressingVersion>(addressingVersion, messageVersion.Addressing);
    }

    [Fact]
    public static void CreateMessageWithSoap12WSAddressing10_WithNoBody()
    {
        var message = Message.CreateMessage(MessageVersion.Soap12WSAddressing10, s_action);
        Assert.Equal<MessageVersion>(MessageVersion.Soap12WSAddressing10, message.Version);
        Assert.Equal<string>(s_action, message.Headers.Action);
        Assert.True(message.IsEmpty);
    }

    [Fact(Skip = "Investigating failure")]
    public static void CreateMessageWithSoap12WSAddressing10_WithBody()
    {
        string content = "This is what goes in the body of the message.";
        object body = content;
        var message = Message.CreateMessage(MessageVersion.Soap12WSAddressing10, s_action, body);

        Assert.Equal<MessageVersion>(MessageVersion.Soap12WSAddressing10, message.Version);
        Assert.Equal<string>(s_action, message.Headers.Action);
        Assert.False(message.IsEmpty);

        var reader = message.GetReaderAtBodyContents();
        var messageBody = reader.ReadContentAsString();

        Assert.Equal<string>(content, messageBody);
    }

    [Fact]
    public static void CreateMessageWithSoap12WSAddressing10_WithCustomBodyWriter()
    {
        var message = Message.CreateMessage(MessageVersion.Soap12WSAddressing10, s_action, new CustomBodyWriter());

        Assert.Equal<MessageVersion>(MessageVersion.Soap12WSAddressing10, message.Version);
        Assert.Equal<string>(s_action, message.Headers.Action);
        Assert.False(message.IsEmpty);

        var reader = message.GetReaderAtBodyContents();
        var messageBody = reader.ReadContentAsString();

        Assert.Equal<string>(string.Empty, messageBody);
    }

    [Fact]
    // Get the MessageVersion from a Custom binding
    public static void GetMessageVersion()
    {
        MessageVersion version = null;
        BindingElement[] bindingElements = new BindingElement[2];
        bindingElements[0] = new TextMessageEncodingBindingElement();
        bindingElements[1] = new HttpTransportBindingElement();
        CustomBinding binding = new CustomBinding(bindingElements);
        version = binding.MessageVersion;

        string expected = "Soap12 (http://www.w3.org/2003/05/soap-envelope) Addressing10 (http://www.w3.org/2005/08/addressing)";
        string actual = version.ToString();
        Assert.Equal<string>(expected, actual);
    }
}
