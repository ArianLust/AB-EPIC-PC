using System;
using System.Runtime.InteropServices;

namespace Rcs
{
	public sealed class RCSSDKPINVOKE
	{
		internal const string LibName = "RCSSDK";

		static RCSSDKPINVOKE()
		{
		}

		[DllImport("RCSSDK")]
		public static extern void SWIGRegisterExceptionCallbacks_RCSSDK(IntPtr applicationDelegate, IntPtr arithmeticDelegate, IntPtr divideByZeroDelegate, IntPtr indexOutOfRangeDelegate, IntPtr invalidCastDelegate, IntPtr invalidOperationDelegate, IntPtr ioDelegate, IntPtr nullReferenceDelegate, IntPtr outOfMemoryDelegate, IntPtr overflowDelegate, IntPtr systemExceptionDelegate);

		[DllImport("RCSSDK")]
		public static extern void SWIGRegisterExceptionArgumentCallbacks_RCSSDK(IntPtr argumentDelegate, IntPtr argumentNullDelegate, IntPtr argumentOutOfRangeDelegate);

		[DllImport("RCSSDK")]
		public static extern void SWIGRegisterStringCallback_RCSSDK(IntPtr stringDelegate);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_VariantDict_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_VariantDict_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint VariantDict_size(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern bool VariantDict_empty(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void VariantDict_Clear(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr VariantDict_getitem(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void VariantDict_setitem(IntPtr jarg1, string jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern bool VariantDict_ContainsKey(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void VariantDict_Add(IntPtr jarg1, string jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern bool VariantDict_Remove(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr VariantDict_create_iterator_begin(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string VariantDict_get_next_key(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void VariantDict_destroy_iterator(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void delete_VariantDict(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_StringDict_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_StringDict_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint StringDict_size(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern bool StringDict_empty(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void StringDict_Clear(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string StringDict_getitem(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void StringDict_setitem(IntPtr jarg1, string jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern bool StringDict_ContainsKey(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void StringDict_Add(IntPtr jarg1, string jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern bool StringDict_Remove(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr StringDict_create_iterator_begin(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string StringDict_get_next_key(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void StringDict_destroy_iterator(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void delete_StringDict(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_NetworkProviderDict_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_NetworkProviderDict_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint NetworkProviderDict_size(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern bool NetworkProviderDict_empty(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void NetworkProviderDict_Clear(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string NetworkProviderDict_getitem(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void NetworkProviderDict_setitem(IntPtr jarg1, int jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern bool NetworkProviderDict_ContainsKey(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void NetworkProviderDict_Add(IntPtr jarg1, int jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern bool NetworkProviderDict_Remove(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr NetworkProviderDict_create_iterator_begin(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern int NetworkProviderDict_get_next_key(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void NetworkProviderDict_destroy_iterator(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void delete_NetworkProviderDict(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_OtherPlayerDict_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_OtherPlayerDict_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint OtherPlayerDict_size(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern bool OtherPlayerDict_empty(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void OtherPlayerDict_Clear(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr OtherPlayerDict_getitem(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void OtherPlayerDict_setitem(IntPtr jarg1, string jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern bool OtherPlayerDict_ContainsKey(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void OtherPlayerDict_Add(IntPtr jarg1, string jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern bool OtherPlayerDict_Remove(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr OtherPlayerDict_create_iterator_begin(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string OtherPlayerDict_get_next_key(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void OtherPlayerDict_destroy_iterator(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void delete_OtherPlayerDict(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void StringList_Clear(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void StringList_Add(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern uint StringList_size(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint StringList_capacity(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void StringList_reserve(IntPtr jarg1, uint jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_StringList_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_StringList_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_StringList_2(int jarg1);

		[DllImport("RCSSDK")]
		public static extern string StringList_getitemcopy(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern string StringList_getitem(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void StringList_setitem(IntPtr jarg1, int jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern void StringList_AddRange(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr StringList_GetRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void StringList_Insert(IntPtr jarg1, int jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern void StringList_InsertRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void StringList_RemoveAt(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void StringList_RemoveRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr StringList_Repeat(string jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void StringList_Reverse_0(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void StringList_Reverse_1(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void StringList_SetRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern bool StringList_Contains(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern int StringList_IndexOf(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern int StringList_LastIndexOf(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern bool StringList_Remove(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void delete_StringList(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void ByteList_Clear(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void ByteList_Add(IntPtr jarg1, byte jarg2);

		[DllImport("RCSSDK")]
		public static extern uint ByteList_size(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint ByteList_capacity(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void ByteList_reserve(IntPtr jarg1, uint jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_ByteList_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_ByteList_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_ByteList_2(int jarg1);

		[DllImport("RCSSDK")]
		public static extern byte ByteList_getitemcopy(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern byte ByteList_getitem(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void ByteList_setitem(IntPtr jarg1, int jarg2, byte jarg3);

		[DllImport("RCSSDK")]
		public static extern void ByteList_AddRange(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr ByteList_GetRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void ByteList_Insert(IntPtr jarg1, int jarg2, byte jarg3);

		[DllImport("RCSSDK")]
		public static extern void ByteList_InsertRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void ByteList_RemoveAt(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void ByteList_RemoveRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr ByteList_Repeat(byte jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void ByteList_Reverse_0(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void ByteList_Reverse_1(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void ByteList_SetRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern bool ByteList_Contains(IntPtr jarg1, byte jarg2);

		[DllImport("RCSSDK")]
		public static extern int ByteList_IndexOf(IntPtr jarg1, byte jarg2);

		[DllImport("RCSSDK")]
		public static extern int ByteList_LastIndexOf(IntPtr jarg1, byte jarg2);

		[DllImport("RCSSDK")]
		public static extern bool ByteList_Remove(IntPtr jarg1, byte jarg2);

		[DllImport("RCSSDK")]
		public static extern void delete_ByteList(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_EventTokensDict_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_EventTokensDict_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint EventTokensDict_size(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern bool EventTokensDict_empty(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void EventTokensDict_Clear(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string EventTokensDict_getitem(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void EventTokensDict_setitem(IntPtr jarg1, int jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern bool EventTokensDict_ContainsKey(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void EventTokensDict_Add(IntPtr jarg1, int jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern bool EventTokensDict_Remove(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr EventTokensDict_create_iterator_begin(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern int EventTokensDict_get_next_key(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void EventTokensDict_destroy_iterator(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void delete_EventTokensDict(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_AssetsInfoDict_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_AssetsInfoDict_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint AssetsInfoDict_size(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern bool AssetsInfoDict_empty(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void AssetsInfoDict_Clear(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr AssetsInfoDict_getitem(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void AssetsInfoDict_setitem(IntPtr jarg1, string jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern bool AssetsInfoDict_ContainsKey(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void AssetsInfoDict_Add(IntPtr jarg1, string jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern bool AssetsInfoDict_Remove(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr AssetsInfoDict_create_iterator_begin(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string AssetsInfoDict_get_next_key(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void AssetsInfoDict_destroy_iterator(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void delete_AssetsInfoDict(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void FlowParticipants_Clear(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void FlowParticipants_Add(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern uint FlowParticipants_size(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint FlowParticipants_capacity(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void FlowParticipants_reserve(IntPtr jarg1, uint jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_FlowParticipants_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_FlowParticipants_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_FlowParticipants_2(int jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr FlowParticipants_getitemcopy(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr FlowParticipants_getitem(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void FlowParticipants_setitem(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void FlowParticipants_AddRange(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr FlowParticipants_GetRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void FlowParticipants_Insert(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void FlowParticipants_InsertRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void FlowParticipants_RemoveAt(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void FlowParticipants_RemoveRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr FlowParticipants_Repeat(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void FlowParticipants_Reverse_0(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void FlowParticipants_Reverse_1(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void FlowParticipants_SetRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void delete_FlowParticipants(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void LeaderboardScores_Clear(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void LeaderboardScores_Add(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern uint LeaderboardScores_size(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint LeaderboardScores_capacity(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void LeaderboardScores_reserve(IntPtr jarg1, uint jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_LeaderboardScores_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_LeaderboardScores_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_LeaderboardScores_2(int jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr LeaderboardScores_getitemcopy(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr LeaderboardScores_getitem(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void LeaderboardScores_setitem(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void LeaderboardScores_AddRange(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr LeaderboardScores_GetRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void LeaderboardScores_Insert(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void LeaderboardScores_InsertRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void LeaderboardScores_RemoveAt(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void LeaderboardScores_RemoveRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr LeaderboardScores_Repeat(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void LeaderboardScores_Reverse_0(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void LeaderboardScores_Reverse_1(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void LeaderboardScores_SetRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void delete_LeaderboardScores(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void LeaderboardResults_Clear(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void LeaderboardResults_Add(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern uint LeaderboardResults_size(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint LeaderboardResults_capacity(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void LeaderboardResults_reserve(IntPtr jarg1, uint jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_LeaderboardResults_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_LeaderboardResults_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_LeaderboardResults_2(int jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr LeaderboardResults_getitemcopy(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr LeaderboardResults_getitem(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void LeaderboardResults_setitem(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void LeaderboardResults_AddRange(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr LeaderboardResults_GetRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void LeaderboardResults_Insert(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void LeaderboardResults_InsertRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void LeaderboardResults_RemoveAt(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void LeaderboardResults_RemoveRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr LeaderboardResults_Repeat(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void LeaderboardResults_Reverse_0(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void LeaderboardResults_Reverse_1(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void LeaderboardResults_SetRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void delete_LeaderboardResults(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Messages_Clear(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Messages_Add(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern uint Messages_size(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint Messages_capacity(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Messages_reserve(IntPtr jarg1, uint jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Messages_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Messages_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Messages_2(int jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Messages_getitemcopy(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr Messages_getitem(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void Messages_setitem(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messages_AddRange(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr Messages_GetRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messages_Insert(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messages_InsertRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messages_RemoveAt(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void Messages_RemoveRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr Messages_Repeat(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void Messages_Reverse_0(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Messages_Reverse_1(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messages_SetRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void delete_Messages(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void MessagingFetchRequests_Clear(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void MessagingFetchRequests_Add(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern uint MessagingFetchRequests_size(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint MessagingFetchRequests_capacity(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void MessagingFetchRequests_reserve(IntPtr jarg1, uint jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_MessagingFetchRequests_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_MessagingFetchRequests_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_MessagingFetchRequests_2(int jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr MessagingFetchRequests_getitemcopy(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr MessagingFetchRequests_getitem(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void MessagingFetchRequests_setitem(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void MessagingFetchRequests_AddRange(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr MessagingFetchRequests_GetRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void MessagingFetchRequests_Insert(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void MessagingFetchRequests_InsertRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void MessagingFetchRequests_RemoveAt(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void MessagingFetchRequests_RemoveRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr MessagingFetchRequests_Repeat(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void MessagingFetchRequests_Reverse_0(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void MessagingFetchRequests_Reverse_1(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void MessagingFetchRequests_SetRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void delete_MessagingFetchRequests(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void MessagingFetchResponses_Clear(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void MessagingFetchResponses_Add(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern uint MessagingFetchResponses_size(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint MessagingFetchResponses_capacity(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void MessagingFetchResponses_reserve(IntPtr jarg1, uint jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_MessagingFetchResponses_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_MessagingFetchResponses_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_MessagingFetchResponses_2(int jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr MessagingFetchResponses_getitemcopy(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr MessagingFetchResponses_getitem(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void MessagingFetchResponses_setitem(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void MessagingFetchResponses_AddRange(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr MessagingFetchResponses_GetRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void MessagingFetchResponses_Insert(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void MessagingFetchResponses_InsertRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void MessagingFetchResponses_RemoveAt(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void MessagingFetchResponses_RemoveRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr MessagingFetchResponses_Repeat(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void MessagingFetchResponses_Reverse_0(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void MessagingFetchResponses_Reverse_1(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void MessagingFetchResponses_SetRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void delete_MessagingFetchResponses(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_MessagingActorPermissionsDict_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_MessagingActorPermissionsDict_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint MessagingActorPermissionsDict_size(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern bool MessagingActorPermissionsDict_empty(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void MessagingActorPermissionsDict_Clear(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern int MessagingActorPermissionsDict_getitem(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void MessagingActorPermissionsDict_setitem(IntPtr jarg1, string jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern bool MessagingActorPermissionsDict_ContainsKey(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void MessagingActorPermissionsDict_Add(IntPtr jarg1, string jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern bool MessagingActorPermissionsDict_Remove(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr MessagingActorPermissionsDict_create_iterator_begin(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string MessagingActorPermissionsDict_get_next_key(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void MessagingActorPermissionsDict_destroy_iterator(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void delete_MessagingActorPermissionsDict(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void WalletVouchers_Clear(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void WalletVouchers_Add(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern uint WalletVouchers_size(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint WalletVouchers_capacity(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void WalletVouchers_reserve(IntPtr jarg1, uint jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_WalletVouchers_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_WalletVouchers_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_WalletVouchers_2(int jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr WalletVouchers_getitemcopy(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr WalletVouchers_getitem(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void WalletVouchers_setitem(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void WalletVouchers_AddRange(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr WalletVouchers_GetRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void WalletVouchers_Insert(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void WalletVouchers_InsertRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void WalletVouchers_RemoveAt(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void WalletVouchers_RemoveRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr WalletVouchers_Repeat(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void WalletVouchers_Reverse_0(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void WalletVouchers_Reverse_1(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void WalletVouchers_SetRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void delete_WalletVouchers(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void CatalogProducts_Clear(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void CatalogProducts_Add(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern uint CatalogProducts_size(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint CatalogProducts_capacity(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void CatalogProducts_reserve(IntPtr jarg1, uint jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_CatalogProducts_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_CatalogProducts_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_CatalogProducts_2(int jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr CatalogProducts_getitemcopy(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr CatalogProducts_getitem(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void CatalogProducts_setitem(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void CatalogProducts_AddRange(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr CatalogProducts_GetRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void CatalogProducts_Insert(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void CatalogProducts_InsertRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void CatalogProducts_RemoveAt(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void CatalogProducts_RemoveRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr CatalogProducts_Repeat(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void CatalogProducts_Reverse_0(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void CatalogProducts_Reverse_1(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void CatalogProducts_SetRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void delete_CatalogProducts(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Users_Clear(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Users_Add(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern uint Users_size(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint Users_capacity(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Users_reserve(IntPtr jarg1, uint jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Users_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Users_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Users_2(int jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Users_getitemcopy(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr Users_getitem(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void Users_setitem(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Users_AddRange(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr Users_GetRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Users_Insert(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Users_InsertRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Users_RemoveAt(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void Users_RemoveRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr Users_Repeat(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void Users_Reverse_0(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Users_Reverse_1(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Users_SetRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void delete_Users(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void SocialNetworkProfiles_Clear(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void SocialNetworkProfiles_Add(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern uint SocialNetworkProfiles_size(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint SocialNetworkProfiles_capacity(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void SocialNetworkProfiles_reserve(IntPtr jarg1, uint jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_SocialNetworkProfiles_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_SocialNetworkProfiles_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_SocialNetworkProfiles_2(int jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr SocialNetworkProfiles_getitemcopy(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr SocialNetworkProfiles_getitem(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void SocialNetworkProfiles_setitem(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialNetworkProfiles_AddRange(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr SocialNetworkProfiles_GetRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialNetworkProfiles_Insert(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialNetworkProfiles_InsertRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialNetworkProfiles_RemoveAt(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void SocialNetworkProfiles_RemoveRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr SocialNetworkProfiles_Repeat(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void SocialNetworkProfiles_Reverse_0(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void SocialNetworkProfiles_Reverse_1(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialNetworkProfiles_SetRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void delete_SocialNetworkProfiles(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void AvatarAssets_Clear(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void AvatarAssets_Add(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern uint AvatarAssets_size(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint AvatarAssets_capacity(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void AvatarAssets_reserve(IntPtr jarg1, uint jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_AvatarAssets_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_AvatarAssets_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_AvatarAssets_2(int jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr AvatarAssets_getitemcopy(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr AvatarAssets_getitem(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void AvatarAssets_setitem(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void AvatarAssets_AddRange(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr AvatarAssets_GetRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void AvatarAssets_Insert(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void AvatarAssets_InsertRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void AvatarAssets_RemoveAt(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void AvatarAssets_RemoveRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr AvatarAssets_Repeat(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void AvatarAssets_Reverse_0(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void AvatarAssets_Reverse_1(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void AvatarAssets_SetRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void delete_AvatarAssets(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void SocialNetworks_Clear(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void SocialNetworks_Add(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern uint SocialNetworks_size(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint SocialNetworks_capacity(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void SocialNetworks_reserve(IntPtr jarg1, uint jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_SocialNetworks_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_SocialNetworks_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_SocialNetworks_2(int jarg1);

		[DllImport("RCSSDK")]
		public static extern int SocialNetworks_getitemcopy(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern int SocialNetworks_getitem(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void SocialNetworks_setitem(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialNetworks_AddRange(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr SocialNetworks_GetRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialNetworks_Insert(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialNetworks_InsertRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialNetworks_RemoveAt(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void SocialNetworks_RemoveRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr SocialNetworks_Repeat(int jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void SocialNetworks_Reverse_0(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void SocialNetworks_Reverse_1(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialNetworks_SetRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void delete_SocialNetworks(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void SocialUser_Clear(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void SocialUser_Add(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern uint SocialUser_size(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint SocialUser_capacity(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void SocialUser_reserve(IntPtr jarg1, uint jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_SocialUser_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_SocialUser_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_SocialUser_2(int jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr SocialUser_getitemcopy(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr SocialUser_getitem(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void SocialUser_setitem(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialUser_AddRange(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr SocialUser_GetRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialUser_Insert(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialUser_InsertRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialUser_RemoveAt(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void SocialUser_RemoveRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr SocialUser_Repeat(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void SocialUser_Reverse_0(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void SocialUser_Reverse_1(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialUser_SetRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void delete_SocialUser(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void SocialServices_Clear(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void SocialServices_Add(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern uint SocialServices_size(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint SocialServices_capacity(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void SocialServices_reserve(IntPtr jarg1, uint jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_SocialServices_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_SocialServices_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_SocialServices_2(int jarg1);

		[DllImport("RCSSDK")]
		public static extern int SocialServices_getitemcopy(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern int SocialServices_getitem(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void SocialServices_setitem(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialServices_AddRange(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr SocialServices_GetRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialServices_Insert(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialServices_InsertRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialServices_RemoveAt(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void SocialServices_RemoveRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr SocialServices_Repeat(int jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void SocialServices_Reverse_0(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void SocialServices_Reverse_1(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialServices_SetRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void delete_SocialServices(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void SocialSharingResponses_Clear(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void SocialSharingResponses_Add(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern uint SocialSharingResponses_size(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint SocialSharingResponses_capacity(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void SocialSharingResponses_reserve(IntPtr jarg1, uint jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_SocialSharingResponses_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_SocialSharingResponses_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_SocialSharingResponses_2(int jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr SocialSharingResponses_getitemcopy(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr SocialSharingResponses_getitem(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void SocialSharingResponses_setitem(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialSharingResponses_AddRange(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr SocialSharingResponses_GetRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialSharingResponses_Insert(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialSharingResponses_InsertRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialSharingResponses_RemoveAt(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void SocialSharingResponses_RemoveRange(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr SocialSharingResponses_Repeat(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void SocialSharingResponses_Reverse_0(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void SocialSharingResponses_Reverse_1(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void SocialSharingResponses_SetRange(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void delete_SocialSharingResponses(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern int RovioSdkMajor_get();

		[DllImport("RCSSDK")]
		public static extern int RovioSdkMinor_get();

		[DllImport("RCSSDK")]
		public static extern int RovioSdkRevision_get();

		[DllImport("RCSSDK")]
		public static extern int RovioSdkHotfix_get();

		[DllImport("RCSSDK")]
		public static extern int RovioSdkVersion_get();

		[DllImport("RCSSDK")]
		public static extern void Version_Major_set(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern int Version_Major_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Version_Minor_set(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern int Version_Minor_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Version_Revision_set(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern int Version_Revision_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Version_Hotfix_set(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern int Version_Hotfix_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Version_String_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Version_String_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Version();

		[DllImport("RCSSDK")]
		public static extern void delete_Version(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr SdkVersion_get();

		[DllImport("RCSSDK")]
		public static extern IntPtr EngineVersion_get();

		[DllImport("RCSSDK")]
		public static extern string Application_ServerProduction_get();

		[DllImport("RCSSDK")]
		public static extern string Application_ServerStaging_get();

		[DllImport("RCSSDK")]
		public static extern string Application_ServerDevelopment_get();

		[DllImport("RCSSDK")]
		public static extern void Application_SetRequestTimeout(int jarg1);

		[DllImport("RCSSDK")]
		public static extern void Application_Initialize_0(string jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void Application_Initialize_1(string jarg1);

		[DllImport("RCSSDK")]
		public static extern void Application_Initialize_2();

		[DllImport("RCSSDK")]
		public static extern void Application_InitializeWithPath_0(string jarg1);

		[DllImport("RCSSDK")]
		public static extern void Application_InitializeWithPath_1();

		[DllImport("RCSSDK")]
		public static extern void Application_Update();

		[DllImport("RCSSDK")]
		public static extern void Application_Activate();

		[DllImport("RCSSDK")]
		public static extern void Application_Suspend();

		[DllImport("RCSSDK")]
		public static extern void Application_UrlOpened_0(string jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void Application_UrlOpened_1(string jarg1);

		[DllImport("RCSSDK")]
		public static extern void Application_Destroy();

		[DllImport("RCSSDK")]
		public static extern void Application_SetLogger(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Application_EnableInternalLogger();

		[DllImport("RCSSDK")]
		public static extern void Application_DisableInternalLogger();

		[DllImport("RCSSDK")]
		public static extern void Application_OverwriteServiceConfiguration(string jarg1, string jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Application();

		[DllImport("RCSSDK")]
		public static extern void delete_Application(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void IdentitySessionParameters_ServerUrl_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string IdentitySessionParameters_ServerUrl_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void IdentitySessionParameters_ClientId_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string IdentitySessionParameters_ClientId_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void IdentitySessionParameters_ClientVersion_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string IdentitySessionParameters_ClientVersion_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void IdentitySessionParameters_ClientSecret_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string IdentitySessionParameters_ClientSecret_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void IdentitySessionParameters_Locale_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string IdentitySessionParameters_Locale_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void IdentitySessionParameters_DistributionChannel_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string IdentitySessionParameters_DistributionChannel_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void IdentitySessionParameters_Definition_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string IdentitySessionParameters_Definition_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void IdentitySessionParameters_BuildId_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string IdentitySessionParameters_BuildId_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_IdentitySessionParameters();

		[DllImport("RCSSDK")]
		public static extern void delete_IdentitySessionParameters(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_IdentitySessionBase(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string IdentitySessionBase_GetAccountId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string IdentitySessionBase_GetSharedAccountId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string IdentitySessionBase_GetAccessTokenString(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr IdentitySessionBase_GetParams(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void IdentitySessionBase_SetProfileField(IntPtr jarg1, string jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void IdentitySessionBase_SetProfileFields(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void IdentitySessionBase_ClearProfileFields(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string IdentitySessionBase_GetProfileFieldsAsJson(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr SwigTools_MakeIdentitySharedPtr(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void SwigTools_FreeIdentitySharedPtr(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr SwigTools_MakeSessionSharedPtr(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void SwigTools_FreeSessionSharedPtr(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr SwigTools_CopySessionSharedPtr(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr SwigTools_DowncastSessionSharedPtr(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr SwigTools_GetSessionPtr(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr SwigTools_GetIdentitySessionBasePtr(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_SwigTools();

		[DllImport("RCSSDK")]
		public static extern void delete_SwigTools(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void RemoveSessionRefreshToken();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_TestDevice(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_TestDevice(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void TestDevice_RegisterDevice(IntPtr jarg1, string jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void TestDevice_UnregisterDevice(IntPtr jarg1, string jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern string TestDevice_GetDeviceName();

		[DllImport("RCSSDK")]
		public static extern void TestDevice_ServerLog(string jarg1, int jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern void TestDevice_OnResultCallback(IntPtr jarg1, IntPtr jarg2, bool jarg3);

		[DllImport("RCSSDK")]
		public static extern void TestDevice_OnResultCallbackSwigExplicitTestDevice(IntPtr jarg1, IntPtr jarg2, bool jarg3);

		[DllImport("RCSSDK")]
		public static extern void TestDevice_director_connect(IntPtr jarg1, IntPtr delegate0);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Variant_0();

		[DllImport("RCSSDK")]
		public static extern void delete_Variant(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Variant_1(string jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Variant_2(bool jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Variant_3(int jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Variant_4(long jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Variant_5(double jarg1);

		[DllImport("RCSSDK")]
		public static extern int Variant_GetVariantType(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Variant_StringValue(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern long Variant_IntValue(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern double Variant_DoubleValue(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern bool Variant_BoolValue(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Variant_6(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_AccessToken_0(string jarg1, long jarg2);

		[DllImport("RCSSDK")]
		public static extern void delete_AccessToken(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_AccessToken_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string AccessToken_GetToken(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern bool AccessToken_IsExpired(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern long AccessToken_ExpiresInMillis(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_NetworkCredentials_0(int jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void delete_NetworkCredentials(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern int NetworkCredentials_GetNetworkProvider(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string NetworkCredentials_GetNetworkName(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string NetworkCredentials_GetCredentials(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_NetworkCredentials_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_GameCenterNetworkCredentialsBuilder();

		[DllImport("RCSSDK")]
		public static extern void delete_GameCenterNetworkCredentialsBuilder(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr GameCenterNetworkCredentialsBuilder_Create(string jarg1, string jarg2, string jarg3, string jarg4, string jarg5, ulong jarg6);

		[DllImport("RCSSDK")]
		public static extern void GameCenterNetworkCredentialsBuilder_Authenticate(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void GameCenterNetworkCredentialsBuilder_OnAuthenticateSuccessCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void GameCenterNetworkCredentialsBuilder_OnAuthenticateSuccessCallbackSwigExplicitGameCenterNetworkCredentialsBuilder(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void GameCenterNetworkCredentialsBuilder_OnAuthenticateFailureCallback(IntPtr jarg1, IntPtr jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern void GameCenterNetworkCredentialsBuilder_OnAuthenticateFailureCallbackSwigExplicitGameCenterNetworkCredentialsBuilder(IntPtr jarg1, IntPtr jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern void GameCenterNetworkCredentialsBuilder_director_connect(IntPtr jarg1, IntPtr delegate0, IntPtr delegate1);

		[DllImport("RCSSDK")]
		public static extern IntPtr DummyNetworkCredentialsBuilder_Create(string jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_DummyNetworkCredentialsBuilder(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr RovioAccountNetworkCredentialsBuilder_Create(string jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void delete_RovioAccountNetworkCredentialsBuilder(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr FacebookNetworkCredentialsBuilder_Create(string jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_FacebookNetworkCredentialsBuilder(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr GoogleNetworkCredentialsBuilder_Create_0(string jarg1, string jarg2, string jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern IntPtr GoogleNetworkCredentialsBuilder_Create_1(string jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void delete_GoogleNetworkCredentialsBuilder(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_PlayerData_0();

		[DllImport("RCSSDK")]
		public static extern void delete_PlayerData(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_PlayerData_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr PlayerData_GetPublic(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr PlayerData_GetPrivate(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern bool PlayerData_SetPublic_0(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern bool PlayerData_SetPublic_1(IntPtr jarg1, string jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern bool PlayerData_SetPrivate_0(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern bool PlayerData_SetPrivate_1(IntPtr jarg1, string jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern void PlayerData_SetBirthday(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string PlayerData_GetBirthday(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void PlayerData_SetGender(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern int PlayerData_GetGender(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Player(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Player_0(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Player_AddNetwork(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Player_RemoveNetwork(IntPtr jarg1, int jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern string Player_GetPlayerId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Player_GetCustomerId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Player_GetNetworks(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Player_GetData(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Player_SetData(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern bool Player_IsMigrated(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Player_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Player_OnFailureCallback(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Player_OnFailureCallbackSwigExplicitPlayer(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Player_OnSuccessCallback(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Player_OnSuccessCallbackSwigExplicitPlayer(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Player_director_connect(IntPtr jarg1, IntPtr delegate0, IntPtr delegate1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_OtherPlayerData_0(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_OtherPlayerData(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_OtherPlayerData_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr OtherPlayerData_GetPublic(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_OtherPlayer_0();

		[DllImport("RCSSDK")]
		public static extern void delete_OtherPlayer(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_OtherPlayer_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string OtherPlayer_GetPlayerId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr OtherPlayer_GetData(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Session(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Session(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Session_RegisterPlayer(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Session_Login(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Session_Restore_0(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Session_Restore_1(IntPtr jarg1, string jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Session_Attach(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern bool Session_HasRestorableSession();

		[DllImport("RCSSDK")]
		public static extern IntPtr Session_GetCurrentPlayer(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Session_FindPlayers(IntPtr jarg1, int jarg2, IntPtr jarg3, IntPtr jarg4, IntPtr jarg5);

		[DllImport("RCSSDK")]
		public static extern string Session_GetRefreshToken(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Session_GetAccessToken(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Session_UpdateAccessToken_0(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Session_UpdateAccessToken_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Session_GetEncodedAppEnv(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern ulong Session_GetSessionId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Session_GetAccountId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Session_GetSharedAccountId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Session_GetAccessTokenString(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Session_GetParams(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Session_OnFailureCallback(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Session_OnFailureCallbackSwigExplicitSession(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Session_OnFindPlayersSuccessCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Session_OnFindPlayersSuccessCallbackSwigExplicitSession(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Session_OnNewSessionSuccessCallback(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Session_OnNewSessionSuccessCallbackSwigExplicitSession(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Session_OnUpdateAccessTokenCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Session_OnUpdateAccessTokenCallbackSwigExplicitSession(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern string Session_OnAttachedTokenUpdateRequestedCallback(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern string Session_OnAttachedTokenUpdateRequestedCallbackSwigExplicitSession(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Session_director_connect(IntPtr jarg1, IntPtr delegate0, IntPtr delegate1, IntPtr delegate2, IntPtr delegate3, IntPtr delegate4);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Ads(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Ads(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Ads_AddPlacement_0(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void Ads_AddPlacement_1(IntPtr jarg1, string jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Ads_AddPlacement_2(IntPtr jarg1, string jarg2, int jarg3, int jarg4, int jarg5, int jarg6);

		[DllImport("RCSSDK")]
		public static extern void Ads_AddPlacementNormalized(IntPtr jarg1, string jarg2, float jarg3, float jarg4, float jarg5, float jarg6);

		[DllImport("RCSSDK")]
		public static extern void Ads_StartSession(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern bool Ads_Show(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void Ads_Hide(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void Ads_SetTargetingParams_0(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Ads_SetTargetingParams_1(IntPtr jarg1, string jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Ads_SetStateChangedHandler(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Ads_SetSizeChangedHandler(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Ads_SetActionInvokedHandler(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Ads_SetRewardResultHandler(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Ads_SetNewContentHandler(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern int Ads_GetState(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void Ads_HandleClick(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void Ads_TrackEvent_0(IntPtr jarg1, string jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Ads_TrackEvent_1(IntPtr jarg1, string jarg2, int jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void Ads_SetTrackingParams(IntPtr jarg1, string jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Ads_OnSizeChangedHandler(IntPtr jarg1, IntPtr jarg2, string jarg3, int jarg4, int jarg5);

		[DllImport("RCSSDK")]
		public static extern void Ads_OnSizeChangedHandlerSwigExplicitAds(IntPtr jarg1, IntPtr jarg2, string jarg3, int jarg4, int jarg5);

		[DllImport("RCSSDK")]
		public static extern bool Ads_OnActionInvokedHandler(IntPtr jarg1, IntPtr jarg2, string jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern bool Ads_OnActionInvokedHandlerSwigExplicitAds(IntPtr jarg1, IntPtr jarg2, string jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern bool Ads_OnRendererHandler(IntPtr jarg1, IntPtr jarg2, string jarg3, string jarg4, IntPtr jarg5);

		[DllImport("RCSSDK")]
		public static extern bool Ads_OnRendererHandlerSwigExplicitAds(IntPtr jarg1, IntPtr jarg2, string jarg3, string jarg4, IntPtr jarg5);

		[DllImport("RCSSDK")]
		public static extern void Ads_OnStateChangedHandler(IntPtr jarg1, IntPtr jarg2, string jarg3, int jarg4);

		[DllImport("RCSSDK")]
		public static extern void Ads_OnStateChangedHandlerSwigExplicitAds(IntPtr jarg1, IntPtr jarg2, string jarg3, int jarg4);

		[DllImport("RCSSDK")]
		public static extern void Ads_OnNewContentHandler(IntPtr jarg1, IntPtr jarg2, string jarg3, int jarg4);

		[DllImport("RCSSDK")]
		public static extern void Ads_OnNewContentHandlerSwigExplicitAds(IntPtr jarg1, IntPtr jarg2, string jarg3, int jarg4);

		[DllImport("RCSSDK")]
		public static extern void Ads_OnRewardResultHandler(IntPtr jarg1, IntPtr jarg2, string jarg3, int jarg4, string jarg5);

		[DllImport("RCSSDK")]
		public static extern void Ads_OnRewardResultHandlerSwigExplicitAds(IntPtr jarg1, IntPtr jarg2, string jarg3, int jarg4, string jarg5);

		[DllImport("RCSSDK")]
		public static extern void Ads_director_connect(IntPtr jarg1, IntPtr delegate0, IntPtr delegate1, IntPtr delegate2, IntPtr delegate3, IntPtr delegate4, IntPtr delegate5);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Analytics(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Analytics(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Analytics_Log_0(IntPtr jarg1, string jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Analytics_Log_1(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void AppTrack_Params_Vendor_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string AppTrack_Params_Vendor_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void AppTrack_Params_PublisherId_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string AppTrack_Params_PublisherId_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void AppTrack_Params_ClientId_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string AppTrack_Params_ClientId_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_AppTrack_Params();

		[DllImport("RCSSDK")]
		public static extern void delete_AppTrack_Params(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_AppTrack(IntPtr jarg1, IntPtr jarg2, bool jarg3);

		[DllImport("RCSSDK")]
		public static extern void delete_AppTrack(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void AppTrack_TrackEvent(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void AppTrack_TrackSale(IntPtr jarg1, string jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern void AppTrack_TrackCustomEvent(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void AppTrack_SetEventTokens(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Assets_Info_Name_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Assets_Info_Name_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Assets_Info_Hash_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Assets_Info_Hash_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Assets_Info_CdnUrl_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Assets_Info_CdnUrl_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Assets_Info_Size_set(IntPtr jarg1, long jarg2);

		[DllImport("RCSSDK")]
		public static extern long Assets_Info_Size_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Assets_Info_ToString(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Assets_Info();

		[DllImport("RCSSDK")]
		public static extern void delete_Assets_Info(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Assets_AssetsConfiguration_SegmentBackend_set(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern int Assets_AssetsConfiguration_SegmentBackend_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Assets_AssetsConfiguration_EnableResume_set(IntPtr jarg1, bool jarg2);

		[DllImport("RCSSDK")]
		public static extern bool Assets_AssetsConfiguration_EnableResume_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Assets_AssetsConfiguration_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Assets_AssetsConfiguration_1(int jarg1, bool jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Assets_AssetsConfiguration_2(int jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Assets_AssetsConfiguration(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Assets_0(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Assets_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Assets_2(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void delete_Assets(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Assets_Get(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void Assets_Load_0(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4, IntPtr jarg5);

		[DllImport("RCSSDK")]
		public static extern void Assets_Load_1(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Assets_Load_2(IntPtr jarg1, IntPtr jarg2, int jarg3, IntPtr jarg4, IntPtr jarg5, IntPtr jarg6);

		[DllImport("RCSSDK")]
		public static extern void Assets_Load_3(IntPtr jarg1, IntPtr jarg2, int jarg3, IntPtr jarg4, IntPtr jarg5);

		[DllImport("RCSSDK")]
		public static extern void Assets_LoadMetadata_0(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Assets_LoadMetadata_1(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern string Assets_GetChecksum(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void Assets_RemoveObsoleteAssets_0(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Assets_RemoveObsoleteAssets_1(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Assets_RemoveObsoleteAssets_2(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Assets_OnRemoveObsoleteAssetsFailedCallback(IntPtr jarg1, IntPtr jarg2, int jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void Assets_OnRemoveObsoleteAssetsFailedCallbackSwigExplicitAssets(IntPtr jarg1, IntPtr jarg2, int jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void Assets_OnProgressCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4, double jarg5, double jarg6);

		[DllImport("RCSSDK")]
		public static extern void Assets_OnProgressCallbackSwigExplicitAssets(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4, double jarg5, double jarg6);

		[DllImport("RCSSDK")]
		public static extern void Assets_OnLoadMetadataSuccessCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Assets_OnLoadMetadataSuccessCallbackSwigExplicitAssets(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Assets_OnSuccessCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Assets_OnSuccessCallbackSwigExplicitAssets(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Assets_OnErrorCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4, int jarg5, string jarg6);

		[DllImport("RCSSDK")]
		public static extern void Assets_OnErrorCallbackSwigExplicitAssets(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4, int jarg5, string jarg6);

		[DllImport("RCSSDK")]
		public static extern void Assets_OnRemoveObsoleteAssetsCompleteCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Assets_OnRemoveObsoleteAssetsCompleteCallbackSwigExplicitAssets(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Assets_director_connect(IntPtr jarg1, IntPtr delegate0, IntPtr delegate1, IntPtr delegate2, IntPtr delegate3, IntPtr delegate4, IntPtr delegate5);

		[DllImport("RCSSDK")]
		public static extern void Flow_Response_Message_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Flow_Response_Message_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Flow_Response_Result_set(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern int Flow_Response_Result_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Flow_Response();

		[DllImport("RCSSDK")]
		public static extern void delete_Flow_Response(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Flow_Participant_AccountId_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Flow_Participant_AccountId_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Flow_Participant_ConnectionState_set(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern int Flow_Participant_ConnectionState_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Flow_Participant_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Flow_Participant_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Flow_Participant(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Flow(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Flow(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Flow_SetConnectionStateChangedHandler(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Flow_SetDataReceivedHandler(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Flow_SetParticipantStateChangedHandler(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Flow_Create(IntPtr jarg1, IntPtr jarg2, long jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Flow_Join(IntPtr jarg1, string jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Flow_SetRecipients(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Flow_ClearRecipients(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Flow_Send(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern string Flow_GetFlowId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Flow_GetParticipants(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern int Flow_GetConnectionState(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Flow_OnJoinFlowCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Flow_OnJoinFlowCallbackSwigExplicitFlow(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Flow_OnConnectionStateChangedHandler(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Flow_OnConnectionStateChangedHandlerSwigExplicitFlow(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Flow_OnCreateFlowCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void Flow_OnCreateFlowCallbackSwigExplicitFlow(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void Flow_OnDataReceivedHandler(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Flow_OnDataReceivedHandlerSwigExplicitFlow(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Flow_OnParticipantStateChangedHandler(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Flow_OnParticipantStateChangedHandlerSwigExplicitFlow(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Flow_OnReachabilityCallback(IntPtr jarg1, IntPtr jarg2, bool jarg3);

		[DllImport("RCSSDK")]
		public static extern void Flow_OnReachabilityCallbackSwigExplicitFlow(IntPtr jarg1, IntPtr jarg2, bool jarg3);

		[DllImport("RCSSDK")]
		public static extern void Flow_director_connect(IntPtr jarg1, IntPtr delegate0, IntPtr delegate1, IntPtr delegate2, IntPtr delegate3, IntPtr delegate4, IntPtr delegate5);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Leaderboard(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Leaderboard(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Leaderboard_SubmitScore(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Leaderboard_SubmitScores(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Leaderboard_FetchScore(IntPtr jarg1, string jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Leaderboard_FetchScores(IntPtr jarg1, IntPtr jarg2, string jarg3, IntPtr jarg4, IntPtr jarg5);

		[DllImport("RCSSDK")]
		public static extern void Leaderboard_Matchmake(IntPtr jarg1, string jarg2, int jarg3, uint jarg4, IntPtr jarg5, IntPtr jarg6);

		[DllImport("RCSSDK")]
		public static extern void Leaderboard_FetchTopScores(IntPtr jarg1, string jarg2, uint jarg3, IntPtr jarg4, IntPtr jarg5);

		[DllImport("RCSSDK")]
		public static extern void Leaderboard_OnScoresFetchedCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Leaderboard_OnScoresFetchedCallbackSwigExplicitLeaderboard(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Leaderboard_OnScoreSubmittedCallback(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Leaderboard_OnScoreSubmittedCallbackSwigExplicitLeaderboard(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Leaderboard_OnScoreFetchedCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Leaderboard_OnScoreFetchedCallbackSwigExplicitLeaderboard(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Leaderboard_OnErrorCallback(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Leaderboard_OnErrorCallbackSwigExplicitLeaderboard(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Leaderboard_Score_0(string jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Leaderboard_Score_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Leaderboard_Score(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern bool LeaderboardScores_EQU(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern bool LeaderboardScores_NEQ(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern bool LeaderboardScores_LT(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern bool LeaderboardScores_LTE(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern bool LeaderboardScores_GT(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern bool LeaderboardScores_GTE(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern string Leaderboard_Score_GetAccountId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Leaderboard_Score_GetLevelName(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Leaderboard_Score_SetPoints(IntPtr jarg1, long jarg2);

		[DllImport("RCSSDK")]
		public static extern long Leaderboard_Score_GetPoints(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Leaderboard_Score_SetProperty(IntPtr jarg1, string jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern bool Leaderboard_Score_HasProperty(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Leaderboard_Score_GetProperty(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr Leaderboard_Score_GetProperties(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Leaderboard_Score_2();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Leaderboard_Score_3(string jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr Leaderboard_Score_FromString(string jarg1);

		[DllImport("RCSSDK")]
		public static extern string Leaderboard_Score_ToString(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Leaderboard_Result(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern long Leaderboard_Result_GetRank(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Leaderboard_Result_GetScore(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Leaderboard_Result_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Leaderboard_Result_1(long jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Leaderboard_Result_2(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Leaderboard_director_connect(IntPtr jarg1, IntPtr delegate0, IntPtr delegate1, IntPtr delegate2, IntPtr delegate3);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_OfflineMatchmaker(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_OfflineMatchmaker(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void OfflineMatchmaker_GetAttributes(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void OfflineMatchmaker_SetAttributes(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void OfflineMatchmaker_MatchUsers_0(IntPtr jarg1, string jarg2, IntPtr jarg3, IntPtr jarg4, int jarg5);

		[DllImport("RCSSDK")]
		public static extern void OfflineMatchmaker_MatchUsers_1(IntPtr jarg1, string jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void OfflineMatchmaker_OnMatchUsersCallback(IntPtr jarg1, IntPtr jarg2, int jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void OfflineMatchmaker_OnMatchUsersCallbackSwigExplicitOfflineMatchmaker(IntPtr jarg1, IntPtr jarg2, int jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void OfflineMatchmaker_OnSetAttributesCallback(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void OfflineMatchmaker_OnSetAttributesCallbackSwigExplicitOfflineMatchmaker(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void OfflineMatchmaker_OnGetAttributesCallback(IntPtr jarg1, IntPtr jarg2, int jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void OfflineMatchmaker_OnGetAttributesCallbackSwigExplicitOfflineMatchmaker(IntPtr jarg1, IntPtr jarg2, int jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void OfflineMatchmaker_director_connect(IntPtr jarg1, IntPtr delegate0, IntPtr delegate1, IntPtr delegate2);

		[DllImport("RCSSDK")]
		public static extern void OnlineMatchmaker_Response_Message_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string OnlineMatchmaker_Response_Message_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void OnlineMatchmaker_Response_Result_set(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern int OnlineMatchmaker_Response_Result_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_OnlineMatchmaker_Response();

		[DllImport("RCSSDK")]
		public static extern void delete_OnlineMatchmaker_Response(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_OnlineMatchmaker(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_OnlineMatchmaker(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void OnlineMatchmaker_JoinLobby(IntPtr jarg1, string jarg2, ulong jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void OnlineMatchmaker_LeaveLobby(IntPtr jarg1, string jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void OnlineMatchmaker_FetchLobbies(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void OnlineMatchmaker_OnLeaveLobbyCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void OnlineMatchmaker_OnLeaveLobbyCallbackSwigExplicitOnlineMatchmaker(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void OnlineMatchmaker_OnFetchLobbiesCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void OnlineMatchmaker_OnFetchLobbiesCallbackSwigExplicitOnlineMatchmaker(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void OnlineMatchmaker_OnJoinLobbyCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4, string jarg5);

		[DllImport("RCSSDK")]
		public static extern void OnlineMatchmaker_OnJoinLobbyCallbackSwigExplicitOnlineMatchmaker(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4, string jarg5);

		[DllImport("RCSSDK")]
		public static extern void OnlineMatchmaker_director_connect(IntPtr jarg1, IntPtr delegate0, IntPtr delegate1, IntPtr delegate2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Message_0(string jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Message_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Message(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Message_GetMessageType(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Message_GetId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Message_GetCreatorId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Message_GetSenderId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Message_GetCursor(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern ulong Message_GetTimestamp(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Message_GetContent(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Message_GetCustom_0(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr Message_GetCustom_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Message_2();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Message_3(string jarg1, string jarg2, string jarg3, string jarg4, string jarg5, string jarg6, ulong jarg7, IntPtr jarg8);

		[DllImport("RCSSDK")]
		public static extern void Message_SetId(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Mailbox(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Mailbox(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern int Mailbox_GetState(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Mailbox_SetStateChangedCallback(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Mailbox_SetMessagesReceivedCallback(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Mailbox_Send(IntPtr jarg1, string jarg2, string jarg3, IntPtr jarg4, IntPtr jarg5);

		[DllImport("RCSSDK")]
		public static extern void Mailbox_Erase(IntPtr jarg1, string jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern IntPtr Mailbox_GetMessages(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Mailbox_Sync(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Mailbox_StartMonitoring(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Mailbox_StopMonitoring(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Mailbox_OnSendErrorCallback(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Mailbox_OnSendErrorCallbackSwigExplicitMailbox(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Mailbox_OnEraseSuccessCallback(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Mailbox_OnEraseSuccessCallbackSwigExplicitMailbox(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Mailbox_OnStateChangedCallback(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Mailbox_OnStateChangedCallbackSwigExplicitMailbox(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Mailbox_OnSendSuccessCallback(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Mailbox_OnSendSuccessCallbackSwigExplicitMailbox(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Mailbox_OnMessagesReceivedCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Mailbox_OnMessagesReceivedCallbackSwigExplicitMailbox(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Mailbox_OnEraseErrorCallback(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Mailbox_OnEraseErrorCallbackSwigExplicitMailbox(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Mailbox_director_connect(IntPtr jarg1, IntPtr delegate0, IntPtr delegate1, IntPtr delegate2, IntPtr delegate3, IntPtr delegate4, IntPtr delegate5);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Messaging_0(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Messaging_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Messaging(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Messaging_CreateActor_0(IntPtr jarg1, string jarg2, IntPtr jarg3, string jarg4, IntPtr jarg5, IntPtr jarg6);

		[DllImport("RCSSDK")]
		public static extern void Messaging_CreateActor_1(IntPtr jarg1, string jarg2, IntPtr jarg3, string jarg4, IntPtr jarg5);

		[DllImport("RCSSDK")]
		public static extern void Messaging_CreateActor_2(IntPtr jarg1, string jarg2, IntPtr jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void Messaging_CreateActor_3(IntPtr jarg1, string jarg2, IntPtr jarg3, string jarg4, ulong jarg5, ulong jarg6, IntPtr jarg7, IntPtr jarg8);

		[DllImport("RCSSDK")]
		public static extern void Messaging_CreateActor_4(IntPtr jarg1, string jarg2, IntPtr jarg3, string jarg4, ulong jarg5, ulong jarg6, IntPtr jarg7);

		[DllImport("RCSSDK")]
		public static extern void Messaging_CreateActor_5(IntPtr jarg1, string jarg2, IntPtr jarg3, string jarg4, ulong jarg5, ulong jarg6);

		[DllImport("RCSSDK")]
		public static extern void Messaging_DeleteActor_0(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Messaging_DeleteActor_1(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_DeleteActor_2(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Messaging_QueryActor_0(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Messaging_QueryActor_1(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_QueryActor_2(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Messaging_ModifyActorPermissions_0(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, string jarg4, IntPtr jarg5, IntPtr jarg6);

		[DllImport("RCSSDK")]
		public static extern void Messaging_ModifyActorPermissions_1(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, string jarg4, IntPtr jarg5);

		[DllImport("RCSSDK")]
		public static extern void Messaging_ModifyActorPermissions_2(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void Messaging_Tell_0(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4, IntPtr jarg5);

		[DllImport("RCSSDK")]
		public static extern void Messaging_Tell_1(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Messaging_Tell_2(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_Tell_3(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4, IntPtr jarg5);

		[DllImport("RCSSDK")]
		public static extern void Messaging_Tell_4(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Messaging_Tell_5(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_Ask_0(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4, IntPtr jarg5, IntPtr jarg6);

		[DllImport("RCSSDK")]
		public static extern void Messaging_Ask_1(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4, IntPtr jarg5);

		[DllImport("RCSSDK")]
		public static extern void Messaging_Ask_2(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Messaging_Ask_3(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_Ask_4(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4, IntPtr jarg5, IntPtr jarg6);

		[DllImport("RCSSDK")]
		public static extern void Messaging_Ask_5(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4, IntPtr jarg5);

		[DllImport("RCSSDK")]
		public static extern void Messaging_Ask_6(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Messaging_Ask_7(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_DeleteMessage_0(IntPtr jarg1, IntPtr jarg2, string jarg3, IntPtr jarg4, IntPtr jarg5);

		[DllImport("RCSSDK")]
		public static extern void Messaging_DeleteMessage_1(IntPtr jarg1, IntPtr jarg2, string jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Messaging_DeleteMessage_2(IntPtr jarg1, IntPtr jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_Fetch_0(IntPtr jarg1, IntPtr jarg2, string jarg3, int jarg4, uint jarg5, IntPtr jarg6, IntPtr jarg7);

		[DllImport("RCSSDK")]
		public static extern void Messaging_Fetch_1(IntPtr jarg1, IntPtr jarg2, string jarg3, int jarg4, uint jarg5, IntPtr jarg6);

		[DllImport("RCSSDK")]
		public static extern void Messaging_Fetch_2(IntPtr jarg1, IntPtr jarg2, string jarg3, int jarg4, uint jarg5);

		[DllImport("RCSSDK")]
		public static extern void Messaging_FetchMany_0(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Messaging_FetchMany_1(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_FetchMany_2(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern string Messaging_GetServiceName(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Messaging_OnMessageSentCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_OnMessageSentCallbackSwigExplicitMessaging(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_OnMessageFetchedCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_OnMessageFetchedCallbackSwigExplicitMessaging(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_OnActorDeletedCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_OnActorDeletedCallbackSwigExplicitMessaging(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_OnMessagesFetchedCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_OnMessagesFetchedCallbackSwigExplicitMessaging(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_OnMessageResponseReceivedCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_OnMessageResponseReceivedCallbackSwigExplicitMessaging(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_OnMessageResponsesReceivedCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_OnMessageResponsesReceivedCallbackSwigExplicitMessaging(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_OnMessagesSentCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_OnMessagesSentCallbackSwigExplicitMessaging(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_OnActorPermissionsModifiedCallback(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Messaging_OnActorPermissionsModifiedCallbackSwigExplicitMessaging(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Messaging_OnActorQueriedCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_OnActorQueriedCallbackSwigExplicitMessaging(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_OnActorCreatedCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_OnActorCreatedCallbackSwigExplicitMessaging(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_OnErrorCallback(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_OnErrorCallbackSwigExplicitMessaging(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_OnMessageDeletedCallback(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Messaging_OnMessageDeletedCallbackSwigExplicitMessaging(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Messaging_ActorHandle_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Messaging_ActorHandle_1(string jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Messaging_ActorHandle_2(string jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Messaging_ActorHandle_3(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Messaging_ActorHandle(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Messaging_ActorHandle_GetActorType(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Messaging_ActorHandle_SetId(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Messaging_ActorHandle_GetId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Messaging_ActorPermissions_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Messaging_ActorPermissions_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Messaging_ActorPermissions(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Messaging_ActorPermissions_SetPermission(IntPtr jarg1, string jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Messaging_ActorPermissions_RemovePermission(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr Messaging_ActorPermissions_GetPermissions(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Messaging_ActorInfo_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Messaging_ActorInfo_1(string jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4, string jarg5, int jarg6);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Messaging_ActorInfo_2(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Messaging_ActorInfo(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Messaging_ActorInfo_GetOwnerAccountId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Messaging_ActorInfo_GetRelations(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Messaging_ActorInfo_GetProperties(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Messaging_ActorInfo_GetPermissions(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Messaging_ActorInfo_GetMetadata(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern int Messaging_ActorInfo_GetMessageCount(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Messaging_FetchRequest_0(IntPtr jarg1, string jarg2, int jarg3, uint jarg4);

		[DllImport("RCSSDK")]
		public static extern IntPtr Messaging_FetchRequest_GetActorHandle(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Messaging_FetchRequest_GetCursor(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern int Messaging_FetchRequest_GetDirection(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern uint Messaging_FetchRequest_GetAmount(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Messaging_FetchRequest_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Messaging_FetchRequest(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Messaging_FetchResponse_GetActorHandle(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Messaging_FetchResponse_GetMessages(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Messaging_FetchResponse_GetErrorMessage(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Messaging_FetchResponse_0(IntPtr jarg1, IntPtr jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Messaging_FetchResponse_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Messaging_FetchResponse(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Messaging_director_connect(IntPtr jarg1, IntPtr delegate0, IntPtr delegate1, IntPtr delegate2, IntPtr delegate3, IntPtr delegate4, IntPtr delegate5, IntPtr delegate6, IntPtr delegate7, IntPtr delegate8, IntPtr delegate9, IntPtr delegate10, IntPtr delegate11);

		[DllImport("RCSSDK")]
		public static extern void PushNotifications_Info_ServiceId_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string PushNotifications_Info_ServiceId_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void PushNotifications_Info_Content_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string PushNotifications_Info_Content_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_PushNotifications_Info();

		[DllImport("RCSSDK")]
		public static extern void delete_PushNotifications_Info(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_PushNotifications(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void delete_PushNotifications(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void PushNotifications_RegisterDevice(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void PushNotifications_UnregisterDevice(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern string PushNotifications_GetDeviceToken(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string PushNotifications_ServiceIdFromRemoteNotification(string jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr PushNotifications_ServiceInfoFromRemoteNotification(string jarg1);

		[DllImport("RCSSDK")]
		public static extern void PushNotifications_OnSuccessCallback(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void PushNotifications_OnSuccessCallbackSwigExplicitPushNotifications(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void PushNotifications_OnErrorCallback(IntPtr jarg1, IntPtr jarg2, int jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void PushNotifications_OnErrorCallbackSwigExplicitPushNotifications(IntPtr jarg1, IntPtr jarg2, int jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void PushNotifications_director_connect(IntPtr jarg1, IntPtr delegate0, IntPtr delegate1);

		[DllImport("RCSSDK")]
		public static extern void delete_Payment(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern bool Payment_IsSupported();

		[DllImport("RCSSDK")]
		public static extern IntPtr Payment_GetProviders();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Payment_0(string jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Payment_1(string jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Payment_2(IntPtr jarg1, int jarg2, string jarg3, string jarg4, bool jarg5);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Payment_3(IntPtr jarg1, int jarg2, string jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Payment_4(IntPtr jarg1, int jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Payment_5(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern int Payment_Initialize(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern bool Payment_IsInitialized(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern bool Payment_IsEnabled(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern int Payment_GetCapabilities(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_GetProviderName(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern int Payment_FetchCatalog_0(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern int Payment_FetchCatalog_1(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern int Payment_FetchCatalog_2(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Payment_GetCachedCatalog_0(string jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr Payment_GetCachedCatalog_1(string jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Payment_GetCatalog(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Payment_GetRewards(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern int Payment_PurchaseProduct_0(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4, IntPtr jarg5);

		[DllImport("RCSSDK")]
		public static extern int Payment_PurchaseProduct_1(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern int Payment_PurchaseProduct_2(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4, out string jarg5, IntPtr jarg6);

		[DllImport("RCSSDK")]
		public static extern int Payment_PurchaseProduct_3(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4, out string jarg5);

		[DllImport("RCSSDK")]
		public static extern int Payment_RestorePurchases_0(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern int Payment_RestorePurchases_1(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern int Payment_RestorePurchases_2(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern int Payment_RestorePurchases_3(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern int Payment_FetchWallet_0(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern int Payment_FetchWallet_1(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern int Payment_FetchWallet_2(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Payment_GetVouchers(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern int Payment_ConsumeVoucher_0(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern int Payment_ConsumeVoucher_1(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern int Payment_ConsumeVoucher_2(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern int Payment_RedeemCode(IntPtr jarg1, string jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern int Payment_VerifyCode(IntPtr jarg1, string jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern int Payment_SendGift_0(IntPtr jarg1, string jarg2, string jarg3, IntPtr jarg4, IntPtr jarg5);

		[DllImport("RCSSDK")]
		public static extern int Payment_SendGift_1(IntPtr jarg1, string jarg2, string jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern int Payment_SendGift_2(IntPtr jarg1, string jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern int Payment_VerifyReward(IntPtr jarg1, string jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern int Payment_ReportReward_0(IntPtr jarg1, string jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern int Payment_ReportReward_1(IntPtr jarg1, string jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern int Payment_ReportReward_2(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern int Payment_DeliverReward_0(IntPtr jarg1, string jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern int Payment_DeliverReward_1(IntPtr jarg1, string jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern int Payment_DeliverReward_2(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void Payment_SetStealthMode(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Payment_OnVerifySuccessCallback(IntPtr jarg1, IntPtr jarg2, string jarg3, int jarg4);

		[DllImport("RCSSDK")]
		public static extern void Payment_OnVerifySuccessCallbackSwigExplicitPayment(IntPtr jarg1, IntPtr jarg2, string jarg3, int jarg4);

		[DllImport("RCSSDK")]
		public static extern void Payment_OnSuccessCallback(IntPtr jarg1, IntPtr jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern void Payment_OnSuccessCallbackSwigExplicitPayment(IntPtr jarg1, IntPtr jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern void Payment_OnProgressCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Payment_OnProgressCallbackSwigExplicitPayment(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Payment_OnPurchaseErrorCallback(IntPtr jarg1, IntPtr jarg2, int jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Payment_OnPurchaseErrorCallbackSwigExplicitPayment(IntPtr jarg1, IntPtr jarg2, int jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Payment_OnRedeemSuccessCallback(IntPtr jarg1, IntPtr jarg2, string jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void Payment_OnRedeemSuccessCallbackSwigExplicitPayment(IntPtr jarg1, IntPtr jarg2, string jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void Payment_OnPurchaseSuccessCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Payment_OnPurchaseSuccessCallbackSwigExplicitPayment(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Payment_OnErrorCallback(IntPtr jarg1, IntPtr jarg2, int jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void Payment_OnErrorCallbackSwigExplicitPayment(IntPtr jarg1, IntPtr jarg2, int jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Payment_Info(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Payment_Info(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern int Payment_Info_GetStatus(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Info_GetTransactionId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Info_GetProductId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Info_GetReceiptId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Info_GetPurchaseId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Info_GetVoucherId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Info_StatusToString(int jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Payment_Product_0(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Payment_Product(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Payment_Product_1(string jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Product_GetId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Product_GetProviderId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern int Payment_Product_GetProductType(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Product_GetToken(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Product_GetName(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Product_GetReferenceName(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Product_GetPrice(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Product_GetUnformattedPrice(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Product_GetCurrencyCode(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Product_GetCountryCode(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern float Payment_Product_GetReferencePrice(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Product_GetDescription(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Payment_Product_GetProviderData(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Product_GetProviderDataString(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Payment_Product_GetClientData(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Product_GetClientDataString(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Product_ToJson(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Product_TypeToString(int jarg1);

		[DllImport("RCSSDK")]
		public static extern void Payment_Product_SetProviderInfo_0(IntPtr jarg1, string jarg2, string jarg3, string jarg4, string jarg5, string jarg6, string jarg7);

		[DllImport("RCSSDK")]
		public static extern void Payment_Product_SetProviderInfo_1(IntPtr jarg1, string jarg2, string jarg3, string jarg4, string jarg5, string jarg6);

		[DllImport("RCSSDK")]
		public static extern void Payment_Product_SetProviderInfo_2(IntPtr jarg1, string jarg2, string jarg3, string jarg4, string jarg5);

		[DllImport("RCSSDK")]
		public static extern void Payment_Product_SetProviderInfo_3(IntPtr jarg1, string jarg2, string jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Payment_Voucher_0(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Payment_Voucher(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Payment_Voucher_1(string jarg1, string jarg2, bool jarg3, string jarg4, int jarg5, string jarg6);

		[DllImport("RCSSDK")]
		public static extern bool Payment_Voucher_IsConsumable(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Voucher_GetId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Voucher_GetProductId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Payment_Voucher_GetClientData(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Voucher_GetClientDataString(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern int Payment_Voucher_GetSourceType(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Voucher_GetSourceId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Payment_Voucher_TypeToString(int jarg1);

		[DllImport("RCSSDK")]
		public static extern void Payment_director_connect(IntPtr jarg1, IntPtr delegate0, IntPtr delegate1, IntPtr delegate2, IntPtr delegate3, IntPtr delegate4, IntPtr delegate5, IntPtr delegate6);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Storage_0(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Storage_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Storage(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Storage_Set_0(IntPtr jarg1, string jarg2, string jarg3, IntPtr jarg4, IntPtr jarg5, IntPtr jarg6, int jarg7);

		[DllImport("RCSSDK")]
		public static extern void Storage_Set_1(IntPtr jarg1, string jarg2, string jarg3, IntPtr jarg4, IntPtr jarg5, IntPtr jarg6);

		[DllImport("RCSSDK")]
		public static extern void Storage_Get_0(IntPtr jarg1, string jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Storage_Get_1(IntPtr jarg1, IntPtr jarg2, string jarg3, IntPtr jarg4, IntPtr jarg5);

		[DllImport("RCSSDK")]
		public static extern void Storage_OnDataGetErrorCallback(IntPtr jarg1, IntPtr jarg2, string jarg3, int jarg4);

		[DllImport("RCSSDK")]
		public static extern void Storage_OnDataGetErrorCallbackSwigExplicitStorage(IntPtr jarg1, IntPtr jarg2, string jarg3, int jarg4);

		[DllImport("RCSSDK")]
		public static extern void Storage_OnDataByAccountIdGetCallback(IntPtr jarg1, IntPtr jarg2, string jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Storage_OnDataByAccountIdGetCallbackSwigExplicitStorage(IntPtr jarg1, IntPtr jarg2, string jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern string Storage_OnDataSetConflictCallback(IntPtr jarg1, IntPtr jarg2, string jarg3, string jarg4, string jarg5);

		[DllImport("RCSSDK")]
		public static extern string Storage_OnDataSetConflictCallbackSwigExplicitStorage(IntPtr jarg1, IntPtr jarg2, string jarg3, string jarg4, string jarg5);

		[DllImport("RCSSDK")]
		public static extern void Storage_OnDataGetCallback(IntPtr jarg1, IntPtr jarg2, string jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void Storage_OnDataGetCallbackSwigExplicitStorage(IntPtr jarg1, IntPtr jarg2, string jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void Storage_OnDataSetFailedCallback(IntPtr jarg1, IntPtr jarg2, string jarg3, int jarg4);

		[DllImport("RCSSDK")]
		public static extern void Storage_OnDataSetFailedCallbackSwigExplicitStorage(IntPtr jarg1, IntPtr jarg2, string jarg3, int jarg4);

		[DllImport("RCSSDK")]
		public static extern void Storage_OnDataSetCallback(IntPtr jarg1, IntPtr jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern void Storage_OnDataSetCallbackSwigExplicitStorage(IntPtr jarg1, IntPtr jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern void Storage_director_connect(IntPtr jarg1, IntPtr delegate0, IntPtr delegate1, IntPtr delegate2, IntPtr delegate3, IntPtr delegate4, IntPtr delegate5);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_NetworkTime(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_NetworkTime(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void NetworkTime_Sync(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern ulong NetworkTime_GetTime(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern bool NetworkTime_IsSync(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void NetworkTime_OnSyncSuccessCallback(IntPtr jarg1, IntPtr jarg2, ulong jarg3);

		[DllImport("RCSSDK")]
		public static extern void NetworkTime_OnSyncSuccessCallbackSwigExplicitNetworkTime(IntPtr jarg1, IntPtr jarg2, ulong jarg3);

		[DllImport("RCSSDK")]
		public static extern void NetworkTime_OnSyncErrorCallback(IntPtr jarg1, IntPtr jarg2, int jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void NetworkTime_OnSyncErrorCallbackSwigExplicitNetworkTime(IntPtr jarg1, IntPtr jarg2, int jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void NetworkTime_director_connect(IntPtr jarg1, IntPtr delegate0, IntPtr delegate1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_AppConfiguration(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_AppConfiguration(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void AppConfiguration_Fetch(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void AppConfiguration_OnSuccessCallback(IntPtr jarg1, IntPtr jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern void AppConfiguration_OnSuccessCallbackSwigExplicitAppConfiguration(IntPtr jarg1, IntPtr jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern void AppConfiguration_OnErrorCallback(IntPtr jarg1, IntPtr jarg2, int jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void AppConfiguration_OnErrorCallbackSwigExplicitAppConfiguration(IntPtr jarg1, IntPtr jarg2, int jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void AppConfiguration_director_connect(IntPtr jarg1, IntPtr delegate0, IntPtr delegate1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_IdentityToSessionMigration(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_IdentityToSessionMigration(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern bool IdentityToSessionMigration_HasMigratableIdentity();

		[DllImport("RCSSDK")]
		public static extern void IdentityToSessionMigration_RestoreMigratableIdentity(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void IdentityToSessionMigration_LoginMigratableIdentity(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void IdentityToSessionMigration_OnFailureCallback(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void IdentityToSessionMigration_OnFailureCallbackSwigExplicitIdentityToSessionMigration(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void IdentityToSessionMigration_OnSuccessCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void IdentityToSessionMigration_OnSuccessCallbackSwigExplicitIdentityToSessionMigration(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void IdentityToSessionMigration_director_connect(IntPtr jarg1, IntPtr delegate0, IntPtr delegate1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_User_SocialNetworkProfile_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_User_SocialNetworkProfile_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string User_SocialNetworkProfile_GetDescription(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void User_SocialNetworkProfile_SocialNetwork_set(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern int User_SocialNetworkProfile_SocialNetwork_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void User_SocialNetworkProfile_Uid_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string User_SocialNetworkProfile_Uid_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void User_SocialNetworkProfile_AvatarUrl_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string User_SocialNetworkProfile_AvatarUrl_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void User_SocialNetworkProfile_Name_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string User_SocialNetworkProfile_Name_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_User_SocialNetworkProfile(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void User_AvatarAsset_AvatarId_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string User_AvatarAsset_AvatarId_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void User_AvatarAsset_AvatarUrl_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string User_AvatarAsset_AvatarUrl_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void User_AvatarAsset_Hash_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string User_AvatarAsset_Hash_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void User_AvatarAsset_Size_set(IntPtr jarg1, ulong jarg2);

		[DllImport("RCSSDK")]
		public static extern ulong User_AvatarAsset_Size_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void User_AvatarAsset_Dimension_set(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern int User_AvatarAsset_Dimension_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_User_AvatarAsset();

		[DllImport("RCSSDK")]
		public static extern void delete_User_AvatarAsset(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string User_GetAccountId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr User_GetSocialNetworkProfiles(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void User_SetSocialNetworkProfiles(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern string User_GetName(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern string User_GetAvatarUrl(IntPtr jarg1, int jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void User_SetGlobalAvatarAssets(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr User_GetGlobalAvatarAssets(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string User_GetDescription(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void User_SetNickName(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_User_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_User_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_User_2(string jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_User(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string UserProfile_ProfileFirstName_get();

		[DllImport("RCSSDK")]
		public static extern string UserProfile_ProfileLastName_get();

		[DllImport("RCSSDK")]
		public static extern string UserProfile_ProfileBirthday_get();

		[DllImport("RCSSDK")]
		public static extern void delete_UserProfile(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_UserProfile_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_UserProfile_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_UserProfile_2(string jarg1, string jarg2, IntPtr jarg3, IntPtr jarg4, IntPtr jarg5, IntPtr jarg6, IntPtr jarg7);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_UserProfile_3(string jarg1, string jarg2, IntPtr jarg3, IntPtr jarg4, IntPtr jarg5, IntPtr jarg6);

		[DllImport("RCSSDK")]
		public static extern string UserProfile_GetSharedAccountId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string UserProfile_GetNickname(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string UserProfile_GetEmailAddress(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string UserProfile_GetAvatar(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void UserProfile_SetAvatarAssets(IntPtr jarg1, int jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr UserProfile_GetAvatarAssetsParameters(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr UserProfile_GetLoggedInSocialNetwork(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr UserProfile_GetConnectedSocialNetworks(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string UserProfile_GetParameter(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void UserProfile_SetParameter(IntPtr jarg1, string jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr UserProfile_GetParameters(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string UserProfile_GetAccountId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr UserProfile_GetFacebookParameters(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Identity(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Identity(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Identity_Login(IntPtr jarg1, int jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Identity_LoginWithUi(IntPtr jarg1, int jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Identity_LoginWithParams(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Identity_Logout(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Identity_FetchAccessToken_0(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Identity_FetchAccessToken_1(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern string Identity_GetConfigurationParameter(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr Identity_GetUserProfile(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Identity_GetUserProfiles(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Identity_ValidateNickname(IntPtr jarg1, string jarg2, bool jarg3, IntPtr jarg4, IntPtr jarg5);

		[DllImport("RCSSDK")]
		public static extern int Identity_GetStatus(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Identity_GetSharedAccountId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Identity_GetAccountId(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Identity_GetNickname(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Identity_GetAvatar(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern bool Identity_IsLoggedIn(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Identity_GetAccessToken(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Identity_GetRefreshToken(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Identity_GetParams(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Identity_GetSegment(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Identity_GetAccessTokenString(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Identity_OnValidateNicknameErrorCallback(IntPtr jarg1, IntPtr jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern void Identity_OnValidateNicknameErrorCallbackSwigExplicitIdentity(IntPtr jarg1, IntPtr jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern void Identity_OnGetUserProfilesSuccessCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Identity_OnGetUserProfilesSuccessCallbackSwigExplicitIdentity(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Identity_OnAccessTokenSuccessCallback(IntPtr jarg1, IntPtr jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern void Identity_OnAccessTokenSuccessCallbackSwigExplicitIdentity(IntPtr jarg1, IntPtr jarg2, string jarg3);

		[DllImport("RCSSDK")]
		public static extern void Identity_OnGetUserProfilesErrorCallback(IntPtr jarg1, IntPtr jarg2, int jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void Identity_OnGetUserProfilesErrorCallbackSwigExplicitIdentity(IntPtr jarg1, IntPtr jarg2, int jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void Identity_OnLoginFailureCallback(IntPtr jarg1, IntPtr jarg2, int jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void Identity_OnLoginFailureCallbackSwigExplicitIdentity(IntPtr jarg1, IntPtr jarg2, int jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void Identity_OnAccessTokenFailureCallback(IntPtr jarg1, IntPtr jarg2, int jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void Identity_OnAccessTokenFailureCallbackSwigExplicitIdentity(IntPtr jarg1, IntPtr jarg2, int jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void Identity_OnLoginSuccessCallback(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Identity_OnLoginSuccessCallbackSwigExplicitIdentity(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Identity_OnValidateNicknameSuccessCallback(IntPtr jarg1, IntPtr jarg2, bool jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void Identity_OnValidateNicknameSuccessCallbackSwigExplicitIdentity(IntPtr jarg1, IntPtr jarg2, bool jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void Identity_director_connect(IntPtr jarg1, IntPtr delegate0, IntPtr delegate1, IntPtr delegate2, IntPtr delegate3, IntPtr delegate4, IntPtr delegate5, IntPtr delegate6, IntPtr delegate7);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_AgeGenderQuery_0(bool jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_AgeGenderQuery_1(bool jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_AgeGenderQuery_2();

		[DllImport("RCSSDK")]
		public static extern void delete_AgeGenderQuery(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void AgeGenderQuery_SetRequiresGender(IntPtr jarg1, bool jarg2);

		[DllImport("RCSSDK")]
		public static extern void AgeGenderQuery_Show(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void AgeGenderQuery_SetTextColor(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void AgeGenderQuery_SetButtonTextColor(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void AgeGenderQuery_SetHighlightColor(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void AgeGenderQuery_OnCancelledCallback(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void AgeGenderQuery_OnCancelledCallbackSwigExplicitAgeGenderQuery(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void AgeGenderQuery_OnCompletedCallback(IntPtr jarg1, IntPtr jarg2, int jarg3, string jarg4, string jarg5);

		[DllImport("RCSSDK")]
		public static extern void AgeGenderQuery_OnCompletedCallbackSwigExplicitAgeGenderQuery(IntPtr jarg1, IntPtr jarg2, int jarg3, string jarg4, string jarg5);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_AgeGenderQuery_Color_0(byte jarg1, byte jarg2, byte jarg3, byte jarg4);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_AgeGenderQuery_Color_1(byte jarg1, byte jarg2, byte jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr AgeGenderQuery_Color_White();

		[DllImport("RCSSDK")]
		public static extern IntPtr AgeGenderQuery_Color_Black();

		[DllImport("RCSSDK")]
		public static extern IntPtr AgeGenderQuery_Color_Red();

		[DllImport("RCSSDK")]
		public static extern IntPtr AgeGenderQuery_Color_Green();

		[DllImport("RCSSDK")]
		public static extern IntPtr AgeGenderQuery_Color_Blue();

		[DllImport("RCSSDK")]
		public static extern IntPtr AgeGenderQuery_Color_Yellow();

		[DllImport("RCSSDK")]
		public static extern void AgeGenderQuery_Color_R_set(IntPtr jarg1, byte jarg2);

		[DllImport("RCSSDK")]
		public static extern byte AgeGenderQuery_Color_R_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void AgeGenderQuery_Color_G_set(IntPtr jarg1, byte jarg2);

		[DllImport("RCSSDK")]
		public static extern byte AgeGenderQuery_Color_G_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void AgeGenderQuery_Color_B_set(IntPtr jarg1, byte jarg2);

		[DllImport("RCSSDK")]
		public static extern byte AgeGenderQuery_Color_B_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void AgeGenderQuery_Color_A_set(IntPtr jarg1, byte jarg2);

		[DllImport("RCSSDK")]
		public static extern byte AgeGenderQuery_Color_A_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_AgeGenderQuery_Color(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void AgeGenderQuery_director_connect(IntPtr jarg1, IntPtr delegate0, IntPtr delegate1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Friends(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void delete_Friends(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern bool Friends_IsInitialized(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Friends_IsConnected(IntPtr jarg1, int jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Friends_Connect(IntPtr jarg1, int jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Friends_Disconnect(IntPtr jarg1, int jarg2, IntPtr jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Friends_GetFriends(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr Friends_GetSocialNetworks(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Friends_AvatarUrl(int jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void Friends_OnConnectSuccessCallback(IntPtr jarg1, IntPtr jarg2, int jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Friends_OnConnectSuccessCallbackSwigExplicitFriends(IntPtr jarg1, IntPtr jarg2, int jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Friends_OnIsConnectedSuccessCallback(IntPtr jarg1, IntPtr jarg2, int jarg3, IntPtr jarg4, IntPtr jarg5);

		[DllImport("RCSSDK")]
		public static extern void Friends_OnIsConnectedSuccessCallbackSwigExplicitFriends(IntPtr jarg1, IntPtr jarg2, int jarg3, IntPtr jarg4, IntPtr jarg5);

		[DllImport("RCSSDK")]
		public static extern void Friends_OnIsConnectedErrorCallback(IntPtr jarg1, IntPtr jarg2, int jarg3, IntPtr jarg4, IntPtr jarg5, int jarg6);

		[DllImport("RCSSDK")]
		public static extern void Friends_OnIsConnectedErrorCallbackSwigExplicitFriends(IntPtr jarg1, IntPtr jarg2, int jarg3, IntPtr jarg4, IntPtr jarg5, int jarg6);

		[DllImport("RCSSDK")]
		public static extern void Friends_OnGetFriendsErrorCallback(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Friends_OnGetFriendsErrorCallbackSwigExplicitFriends(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Friends_OnDisconnectSuccessCallback(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Friends_OnDisconnectSuccessCallbackSwigExplicitFriends(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Friends_OnGetFriendsSuccessCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Friends_OnGetFriendsSuccessCallbackSwigExplicitFriends(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Friends_OnDisconnectErrorCallback(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Friends_OnDisconnectErrorCallbackSwigExplicitFriends(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Friends_OnConnectErrorCallback(IntPtr jarg1, IntPtr jarg2, int jarg3, int jarg4);

		[DllImport("RCSSDK")]
		public static extern void Friends_OnConnectErrorCallbackSwigExplicitFriends(IntPtr jarg1, IntPtr jarg2, int jarg3, int jarg4);

		[DllImport("RCSSDK")]
		public static extern void Friends_director_connect(IntPtr jarg1, IntPtr delegate0, IntPtr delegate1, IntPtr delegate2, IntPtr delegate3, IntPtr delegate4, IntPtr delegate5, IntPtr delegate6, IntPtr delegate7);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_FriendsCache(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_FriendsCache(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void FriendsCache_Initialize(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr FriendsCache_GetFriends(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr FriendsCache_GetFriend(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr FriendsCache_GetSocialNetworkFriends_0(IntPtr jarg1, int jarg2, ulong jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr FriendsCache_GetSocialNetworkFriends_1(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr FriendsCache_GetBackend(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void FriendsCache_OnRefreshedCallback(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void FriendsCache_OnRefreshedCallbackSwigExplicitFriendsCache(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void FriendsCache_director_connect(IntPtr jarg1, IntPtr delegate0);

		[DllImport("RCSSDK")]
		public static extern string FacebookService_get();

		[DllImport("RCSSDK")]
		public static extern string OtherService_get();

		[DllImport("RCSSDK")]
		public static extern string PlatformService_get();

		[DllImport("RCSSDK")]
		public static extern string DigitsService_get();

		[DllImport("RCSSDK")]
		public static extern void delete_Social(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Social_getInstance_private();

		[DllImport("RCSSDK")]
		public static extern void Social_Configure(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern int Social_GetNumOfServices(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Social_GetServices(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_Login(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Social_Logout(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern bool Social_IsLoggedIn(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern void Social_Share(IntPtr jarg1, IntPtr jarg2, int jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Social_GetUserProfile(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Social_GetFriends(IntPtr jarg1, IntPtr jarg2, int jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Social_SendAppRequest(IntPtr jarg1, IntPtr jarg2, int jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern void Social_SendAppInviteRequest(IntPtr jarg1, IntPtr jarg2, int jarg3, IntPtr jarg4);

		[DllImport("RCSSDK")]
		public static extern bool Social_OnOpenUrl_0(IntPtr jarg1, string jarg2, IntPtr jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern bool Social_OnOpenUrl_1(IntPtr jarg1, string jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Social_OnActivate(IntPtr jarg1, bool jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr Social_GetSocialNetworkGlobalParameters(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_SetSocialNetworkGlobalParameters(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern string Social_GetServiceName(int jarg1);

		[DllImport("RCSSDK")]
		public static extern int Social_GetServiceByName(string jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_OnLoginCallback(IntPtr jarg1, IntPtr jarg2, bool jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void Social_OnLoginCallbackSwigExplicitSocial(IntPtr jarg1, IntPtr jarg2, bool jarg3, string jarg4);

		[DllImport("RCSSDK")]
		public static extern void Social_OnAppRequestCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Social_OnAppRequestCallbackSwigExplicitSocial(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Social_OnSharingStartCallback(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Social_OnSharingStartCallbackSwigExplicitSocial(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Social_OnGetFriendsCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Social_OnGetFriendsCallbackSwigExplicitSocial(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Social_OnSharingCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Social_OnSharingCallbackSwigExplicitSocial(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Social_OnSharingAggregatedCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Social_OnSharingAggregatedCallbackSwigExplicitSocial(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Social_OnGetUserProfileCallback(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Social_OnGetUserProfileCallbackSwigExplicitSocial(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern string Social_User_ToString(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Social_User(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_User_UserId_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Social_User_UserId_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_User_UserName_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Social_User_UserName_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_User_Name_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Social_User_Name_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_User_ProfileImageUrl_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Social_User_ProfileImageUrl_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_User_CustomParams_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr Social_User_CustomParams_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Social_User();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Social_Response_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Social_Response_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Social_Response(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Social_Response_ToString(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_Response_Result_set(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern int Social_Response_Result_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_Response_Service_set(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern int Social_Response_Service_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_Response_SocialNetworkReturnCode_set(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern int Social_Response_SocialNetworkReturnCode_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_Response_SocialNetworkMessage_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Social_Response_SocialNetworkMessage_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Social_GetUserProfileResponse_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Social_GetUserProfileResponse_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Social_GetUserProfileResponse_ToString(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Social_GetUserProfileResponse(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_GetUserProfileResponse_UserProfile_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr Social_GetUserProfileResponse_UserProfile_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_GetUserProfileResponse_AccessToken_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Social_GetUserProfileResponse_AccessToken_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_GetUserProfileResponse_AppId_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Social_GetUserProfileResponse_AppId_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Social_SharingRequest();

		[DllImport("RCSSDK")]
		public static extern void delete_Social_SharingRequest(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_SharingRequest_SharingType_set(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern int Social_SharingRequest_SharingType_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_SharingRequest_Title_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Social_SharingRequest_Title_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_SharingRequest_Text_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Social_SharingRequest_Text_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_SharingRequest_ImageUrl_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Social_SharingRequest_ImageUrl_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_SharingRequest_Url_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Social_SharingRequest_Url_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Social_SharingResponse_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Social_SharingResponse_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Social_SharingResponse_ToString(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Social_SharingResponse(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_SharingResponse_SharedPostId_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Social_SharingResponse_SharedPostId_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Social_GetFriendsRequest();

		[DllImport("RCSSDK")]
		public static extern void Social_GetFriendsRequest_FriendsType_set(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern int Social_GetFriendsRequest_FriendsType_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_GetFriendsRequest_Pagination_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Social_GetFriendsRequest_Pagination_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Social_GetFriendsRequest(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Social_GetFriendsResponse_0();

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Social_GetFriendsResponse_1(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern string Social_GetFriendsResponse_ToString(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Social_GetFriendsResponse(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_GetFriendsResponse_Friends_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr Social_GetFriendsResponse_Friends_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_GetFriendsResponse_NextPage_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Social_GetFriendsResponse_NextPage_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Social_AppRequest();

		[DllImport("RCSSDK")]
		public static extern void Social_AppRequest_InteractionMode_set(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern int Social_AppRequest_InteractionMode_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_AppRequest_UserIds_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr Social_AppRequest_UserIds_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_AppRequest_Title_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Social_AppRequest_Title_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_AppRequest_Message_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Social_AppRequest_Message_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_AppRequest_CustomParams_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr Social_AppRequest_CustomParams_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Social_AppRequest(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Social_AppInviteRequest();

		[DllImport("RCSSDK")]
		public static extern void Social_AppInviteRequest_AppLinkUrl_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Social_AppInviteRequest_AppLinkUrl_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_AppInviteRequest_PreviewImageUrl_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Social_AppInviteRequest_PreviewImageUrl_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Social_AppInviteRequest(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Social_AppLinkData();

		[DllImport("RCSSDK")]
		public static extern void Social_AppLinkData_Type_set(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern int Social_AppLinkData_Type_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_AppLinkData_BaseUrl_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Social_AppLinkData_BaseUrl_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Social_AppLinkData(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Social_director_connect(IntPtr jarg1, IntPtr delegate0, IntPtr delegate1, IntPtr delegate2, IntPtr delegate3, IntPtr delegate4, IntPtr delegate5, IntPtr delegate6);

		[DllImport("RCSSDK")]
		public static extern int SocialNetworkReturnCodeDefaultValue_get();

		[DllImport("RCSSDK")]
		public static extern string DefaultChannelGroupId_get();

		[DllImport("RCSSDK")]
		public static extern void Channel_Params_Locale_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Channel_Params_Locale_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Channel_Params_Width_set(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern int Channel_Params_Width_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Channel_Params_Height_set(IntPtr jarg1, int jarg2);

		[DllImport("RCSSDK")]
		public static extern int Channel_Params_Height_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Channel_Params_PushNotificationContent_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Channel_Params_PushNotificationContent_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Channel_Params_EntryPoint_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Channel_Params_EntryPoint_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Channel_Params_SeparateVideoplayerActivityOnAndroid_set(IntPtr jarg1, bool jarg2);

		[DllImport("RCSSDK")]
		public static extern bool Channel_Params_SeparateVideoplayerActivityOnAndroid_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Channel_Params_GroupId_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Channel_Params_GroupId_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Channel_Params_ChannelId_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Channel_Params_ChannelId_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Channel_Params_VideoId_set(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern string Channel_Params_VideoId_get(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Channel_Params();

		[DllImport("RCSSDK")]
		public static extern void delete_Channel_Params(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Channel_0(IntPtr jarg1, string jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Channel_1(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern IntPtr new_Channel_2(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void delete_Channel(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Channel_SetClientId(IntPtr jarg1, string jarg2);

		[DllImport("RCSSDK")]
		public static extern void Channel_SetChannelAudioCallback(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Channel_SetDefaultAdsActionHandler(IntPtr jarg1, IntPtr jarg2);

		[DllImport("RCSSDK")]
		public static extern void Channel_OpenChannelView(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("RCSSDK")]
		public static extern void Channel_CancelChannelViewLoading(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern bool Channel_IsChannelViewOpened(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Channel_NavigateBackChannelView(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern void Channel_LoadFromUrl_0(IntPtr jarg1, string jarg2, int jarg3, int jarg4, string jarg5, string jarg6, string jarg7, bool jarg8, string jarg9, string jarg10);

		[DllImport("RCSSDK")]
		public static extern void Channel_LoadFromUrl_1(IntPtr jarg1, string jarg2, int jarg3, int jarg4, string jarg5, string jarg6, string jarg7, bool jarg8, string jarg9);

		[DllImport("RCSSDK")]
		public static extern void Channel_LoadFromUrl_2(IntPtr jarg1, string jarg2, int jarg3, int jarg4, string jarg5, string jarg6, string jarg7, bool jarg8);

		[DllImport("RCSSDK")]
		public static extern void Channel_LoadFromUrl_3(IntPtr jarg1, string jarg2, int jarg3, int jarg4, string jarg5, string jarg6, string jarg7);

		[DllImport("RCSSDK")]
		public static extern void Channel_LoadFromUrl_4(IntPtr jarg1, string jarg2, int jarg3, int jarg4, string jarg5, string jarg6);

		[DllImport("RCSSDK")]
		public static extern void Channel_LoadFromUrl_5(IntPtr jarg1, string jarg2, int jarg3, int jarg4, string jarg5);

		[DllImport("RCSSDK")]
		public static extern void Channel_LoadFromUrl_6(IntPtr jarg1, string jarg2, int jarg3, int jarg4);

		[DllImport("RCSSDK")]
		public static extern void Channel_OnChannelAudioHandler(IntPtr jarg1, IntPtr jarg2, int jarg3, string jarg4, string jarg5);

		[DllImport("RCSSDK")]
		public static extern void Channel_OnChannelAudioHandlerSwigExplicitChannel(IntPtr jarg1, IntPtr jarg2, int jarg3, string jarg4, string jarg5);

		[DllImport("RCSSDK")]
		public static extern void Channel_OnChannelLoadedHandler(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Channel_OnChannelLoadedHandlerSwigExplicitChannel(IntPtr jarg1, IntPtr jarg2, int jarg3);

		[DllImport("RCSSDK")]
		public static extern void Channel_director_connect(IntPtr jarg1, IntPtr delegate0, IntPtr delegate1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Session_Upcast(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Identity_Upcast(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Social_GetUserProfileResponse_Upcast(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Social_SharingResponse_Upcast(IntPtr jarg1);

		[DllImport("RCSSDK")]
		public static extern IntPtr Social_GetFriendsResponse_Upcast(IntPtr jarg1);
	}
}
