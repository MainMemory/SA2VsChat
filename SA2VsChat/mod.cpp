#include "pch.h"
#include <cstdio>
#include <random>
#include "..\sa2-mod-loader\SA2ModLoader\include\SA2ModLoader.h"

std::random_device rd;
std::default_random_engine gen(rd());

DataPointer(int, StageMessageCount, 0xB5D200);
ObjectMaster* LoadOmochao(NJS_VECTOR* position)
{
	ObjectMaster* obj = AllocateObjectMaster(Omochao_Main, 2, "ObjectMessenger");
	if (obj)
	{
		EntityData1* v7 = AllocateEntityData1();
		if (v7)
		{
			obj->Data1.Entity = v7;
			void* v10 = AllocateEntityData2();
			if (v10)
			{
				obj->EntityData2 = (UnknownData2*)v10;
				v7->Position = *position;
				v7->Scale.x = (float)(gen() % StageMessageCount); // select a random hint from the level's hint message file
				v7->Scale.y = 15;
				v7->Scale.z = 0;
				v7->NextAction |= 3; // force Omochao to follow the player
				v7->Action = 5; // force Omochao into the talking action
				return obj;
			}
			else
				DeleteObject_(obj);
		}
		else
			DeleteObject_(obj);
	}
	return nullptr;
}

void CheckLoadOmochao(ObjectMaster* obj)
{
	if (*(void**)0xB5838C) // make sure Omochao's textures are loaded before spawning
	{
		LoadOmochao(&MainCharObj1[0]->Position);
		DeleteObject_(obj);
	}
}

extern "C"
{
	__declspec(dllexport) void GiveItem(int item)
	{
		if (MainCharObj2[0] && !(MainCharObj2[0]->Powerups & Powerups_Dead))
		{
			DisplayItemBoxItem(0, ItemBox_Items[item].Texture);
			ItemBox_Items[item].Code(MainCharacter[0], 0);
		}
	}

	__declspec(dllexport) void SpawnOmochao()
	{
		if (GameState == GameStates_Ingame && CurrentLevel < LevelIDs_Route101280)
			AllocateObjectMaster(CheckLoadOmochao, 2, "CheckLoadOmochao");
	}

	__declspec(dllexport) void PlayVoice(int id)
	{
		PlayVoice(0, id);
	}

	__declspec(dllexport) void Stop()
	{
		if (MainCharObj2[0] && !(MainCharObj2[0]->Powerups & Powerups_Dead))
		{
			MainCharObj2[0]->Speed.x = 0;
			MainCharObj2[0]->Speed.y = 0;
			MainCharObj2[0]->Speed.z = 0;
		}
	}

	__declspec(dllexport) void GottaGoFast()
	{
		if (MainCharObj2[0] && !(MainCharObj2[0]->Powerups & Powerups_Dead))
			MainCharObj2[0]->Speed.x = MainCharObj2[0]->PhysData.HSpeedCap;
	}

	__declspec(dllexport) void TsafOgAttog()
	{
		if (MainCharObj2[0] && !(MainCharObj2[0]->Powerups & Powerups_Dead))
			MainCharObj2[0]->Speed.x = -MainCharObj2[0]->PhysData.HSpeedCap;
	}

	__declspec(dllexport) void SuperJump()
	{
		if (MainCharObj2[0] && !(MainCharObj2[0]->Powerups & Powerups_Dead))
			MainCharObj2[0]->Speed.y = MainCharObj2[0]->PhysData.VSpeedCap;
	}

	__declspec(dllexport) void TimeStop()
	{
		TimeStopped ^= 2;
	}

	__declspec(dllexport) void Die(const char *user)
	{
		if (TimerMinutes >= 1 && MainCharObj2[0] && !(MainCharObj2[0]->Powerups & Powerups_Dead))
		{
			PrintDebug("Killed by %s!", user);
			KillPlayer(0);
		}
	}

	__declspec(dllexport) void Win(const char *user)
	{
		if (TimerMinutes >= 1 && MainCharObj2[0] && !(MainCharObj2[0]->Powerups & Powerups_Dead))
		{
			PrintDebug("Level ended by %s!", user);
			AwardWin(0);
		}
	}

	__declspec(dllexport) void Grow()
	{
		if (MainCharObj1[0])
		{
			MainCharObj1[0]->Scale.x *= 2;
			MainCharObj1[0]->Scale.y *= 2;
			MainCharObj1[0]->Scale.z *= 2;
		}
	}

	__declspec(dllexport) void Shrink()
	{
		if (MainCharObj1[0])
		{
			MainCharObj1[0]->Scale.x /= 2;
			MainCharObj1[0]->Scale.y /= 2;
			MainCharObj1[0]->Scale.z /= 2;
		}
	}

	__declspec(dllexport) void Bonus(int scr)
	{
		DispTechniqueScore_Load(scr);
	}

	__declspec(dllexport) void Init(const char* path, const HelperFunctions& helperFunctions)
	{
		char buf[MAX_PATH];
		GetCurrentDirectoryA(MAX_PATH, buf);
		SetCurrentDirectoryA(path);
		HMODULE hm = LoadLibraryA("SA2VsChatNET.dll");
		if (hm == NULL)
		{
			SetCurrentDirectoryA(buf);
			PrintDebug("SA2VsChat: Failed to load helper DLL, mod will not function!");
			return;
		}
		GetProcAddress(hm, "TwitchIRCStart")();
		SetCurrentDirectoryA(buf);
	}

	__declspec(dllexport) ModInfo SA2ModInfo { ModLoaderVer };
}