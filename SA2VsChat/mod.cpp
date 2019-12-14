#include "pch.h"
#include <cstdio>
#include "..\sa2-mod-loader\SA2ModLoader\include\SA2ModLoader.h"

extern "C"
{
	__declspec(dllexport) void GiveItem(int item)
	{
		if (MainCharObj2[0] && !(MainCharObj2[0]->Powerups & Powerups_Dead))
		{
			DisplayItemBoxItem(0, ItemBox_Items[item].Texture);
			ItemBox_Items[item].Code(nullptr, 0);
		}
	}

	__declspec(dllexport) void Init(const char* path, const HelperFunctions& helperFunctions)
	{
		char buf[MAX_PATH];
		GetCurrentDirectoryA(MAX_PATH, buf);
		SetCurrentDirectoryA(path);
		HMODULE hm = LoadLibraryA(buf);
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