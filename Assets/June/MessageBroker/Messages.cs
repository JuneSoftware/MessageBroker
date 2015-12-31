using UnityEngine;
using System.Collections;
namespace June {
	/// <summary>
	/// Messages.
	/// </summary>
	public class Messages {

		#if UNITY_EDITOR
		public static string[] ALL_MESSAGES = {
			AppCoinsHudRefresh,
			AppToBackground,
			AppToForeground,
			HomePlayTap,
			HomeSoundTap,
			HomeSettingsTap,
			HomeStoreTap,
			HomeLeaderboardTap,
			RaceStart,
			RaceEnd,
			RaceWon,
			RaceLost,
			RaceDeath,
			RaceHit,
			PodiumHomeTap,
			PodiumReplay,
			PodiumStoreTap,
			PodiumRateUsShown,
			PodiumRateUsOk,
			PodiumRateUsCancel,
			StoreHomeTap,
			StoreCharacterTabTap,
			StoreWeaponTabTap,
			StoreCoinHUDTap
		};
		#endif

		public const string PlayerRegistered = "//app/player_registered";
		public const string AppToBackground = "//app/to_background";
		public const string AppToForeground = "//app/to_foreground";
		public const string AppInAppPurchase = "//app/purchase/in_app";
		public const string AppInGamePurchase = "//app/purchase/in_game";
		public const string AppCoinsHudRefresh = "//app/coins_hud_refresh";
		public const string AppFacebookConnected = "//app/facebook_connected";

		public const string GotEverything = "//app/got_everything";
		public const string GotAllWeapons = "//app/got_all_weapons";
		public const string GotAllCharacters = "//app/got_all_characters";

		public const string PlayerCharacterSelected = "//player/character_select";
		public const string PlayerWeaponSelected = "//player/weapon_selected";

		public const string HomePlayTap = "//home/play_tap";
		public const string HomeSoundTap = "//home/sound_tap";
		public const string HomeSettingsTap = "//home/setting_tap";
		public const string HomeStoreTap = "//home/store_tap";
		public const string HomeLeaderboardTap = "//home/leaderboard_tap";
		public const string HomeFreeCoinsTap = "//home/freecoins_tap";
		public const string HomeFreeCoinsVideoShown = "//home/freecoins_video_shown";

		public const string SettingsNameChangeTap = "//settings/name_change_tap";
		public const string SettingsLanguageTap = "//settings/language_tap";
		public const string SettingsSoundTap = "//settings/sound_tap";
		public const string SettingsMusicTap = "//settings/music_tap";
		public const string SettingsLanguageSelected = "//settings/language_selected";
		public const string SettingsCloseTap = "//settings/close_tap";
		public const string SettingsContactUsTap = "//settings/contact_us_tap";
		public const string SettingsCreditsTap = "//settings/credits_tap";
		public const string SettingsRateUsTap = "//settings/rate_us_tap";
		public const string SettingsRestorePurchaseTap = "//settings/restore_purchase_tap";

		public const string LeaderboardAllTimeTap = "//leaderboard/all_time_tap";
		public const string LeaderboardDailyTap = "//leaderboard/daily_tap";
		public const string LeaderboardWeeklyTap = "//leaderboard/weekly_tap";
		public const string LeaderboardFriendsTap = "//leaderboard/friends_tap";
		public const string LeaderboardCloseTap = "//leaderboard/close_tap";

		public const string RaceStart = "//race/start";
		public const string RaceEnd = "//race/end";
		public const string RaceWon = "//race/won";
		public const string RaceLost = "//race/lost";
		public const string RaceDeath = "//race/death";
		public const string RaceHit = "//race/hit";

		public const string Played5Races = "//race/played_5_races";
		public const string Played10Races = "//race/played_10_races";
		public const string Played25Races = "//race/played_25_races";
		public const string Played50Races = "//race/played_50_races";
		public const string Played100Races = "//race/played_100_races";

		public const string RacePauseTap = "//race/pause_tap";
		public const string RacePauseQuitTap = "//race/pause_quit_tap";
		public const string RacePauseResumeTap = "//race/pause_resume_tap";

		public const string PodiumHomeTap = "//podium/home_tap";
		public const string PodiumReplay = "//podium/replay";
		public const string PodiumStoreTap = "//podium/store_tap";
		public const string PodiumRateUsShown = "//podium/rate_us_shown";
		public const string PodiumRateUsOk = "//podium/rate_us_ok";
		public const string PodiumRateUsCancel = "//podium/rate_us_cancel";

		public const string ImpulseItemUnlocked = "//podium/impulse_item_unlocked";
		public const string ImpulseItemToUnlockShown = "//podium/impulse_item_to_unlock_shown";
		public const string ImpulseItemBuyNowTap = "//podium/impulse_item_buy_now_tap";
		public const string ImpulseItemPurchase = "//podium/impulse_item_purchase";

		public const string StoreHomeTap = "//store/home_tap";
		public const string StoreCharacterTabTap = "//store/character_tab_tap";
		public const string StoreWeaponTabTap = "//store/weapon_tab_tap";
		public const string StoreSpecialTabTap = "//store/special_tab_tap";
		public const string StoreCoinHUDTap = "//store/coin_hud_tap";
		public const string StoreCharacterTap = "//store/character_tap";
		public const string StoreWeaponTap = "//store/weapon_tap";
		public const string StoreSpecialTap = "//store/special_tap";

		public const string Coin2xPurchased = "//store/coin_2x_Purchased";
		public const string Coin100PackPurchased = "//store/coin_100_pack_purchased";
		public const string Coin1800Purchased = "//store/coin_1800_pack_purchased";
		public const string Coin4000Purchased = "//store/coin_4000_pack_purchased";

		public const string StoreCharacterPurchaseTap = "//store/character_purchase_tap";
		public const string StoreWeaponPurchaseTap = "//store/weapon_purchase_tap";

		public const string StoreCharacterPurchased = "//store/character_purchased";
		public const string StoreWeaponPurchased= "//store/weapon_purchased";

		public const string MissionCompleted = "//mission/complete";
		public const string MissionCompletedGiftShown = "//mission/gift_shown";
		public const string MissionCompletedGiftTapped = "//mission/gift_tapped";
		public const string MissionSpunWon = "//mission/misision_spun_won";
		public const string FreeSpunWon = "//mission/free_spun_won";
		public const string CoinsExchangedWon = "//mission/coins_exchanged_won";

		public const string SpinTap = "//spin/spin_tap";
		public const string FirstSpinReward = "//spin/first_spin_reward";

		public const string VideoAdsCoinsGiven = "//video/video_ads_coins_given";
		public const string EndScreenVideoAdBtnShown = "//video/end_screen_videoad_btn_shown";
		public const string EndScreenVideoAdBtnTapped = "//video/end_screen_videoad_btn_tapped";
		public const string EndScreenVideoAdCompleted = "//video/end_screen_videoad_completed";

		public const string HomeFacebookButtonTap = "//facebook/home_facebook_button_tap";
		public const string HomePlayerHudTap = "//facebook/home_player_hud_tap";
		public const string StoreFacebookButtonTap = "//facebook/store_facebook_button_tap";
		public const string FacebookShareButtonTap = "//facebook/facebook_share_button_tap";
		public const string FacebookConnectButtonTap = "//facebook/facebook_connect_button_tap";

		public const string FacebookShareComplete = "//facebook/facebook_share_complete";
		public const string FacebookShareFailed = "//facebook/facebook_share_failed";
		public const string FacebookConnectSuccess = "//facebook/facebook_connect_complete";
		public const string FacebookConnectFailed = "//facebook/facebook_connect_failed";

		public const string RentPopupClosed = "//rent/rent_popup_closed";
		public const string RentPopupShown = "//rent/rent_popup_shown";
		public const string RentSuccess = "//rent/rent_success";
		public const string RentFail = "//rent/rent_fail";
		public const string StoreRentButtonTap = "//rent/store_rent_button_tap";
	}
}