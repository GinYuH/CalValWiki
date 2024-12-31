﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.UI;
using Terraria.GameContent;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Input;

namespace CalValWiki
{
    public class WikiUI : UIState
    {
        public static List<ButtonType> buttons = new List<ButtonType>() { };
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.gameMenu)
                return;
            //if (!WikiPlayer.InUI)
               // return;
            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                Main.blockInput = false;
                WikiPlayer.InUI = false;
                return;
            }
            base.Draw(spriteBatch);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            int startX = Main.screenWidth / 10;
            int startY = Main.screenHeight / 2;
            int buttonSize = 40;
            int spacing = 30;
            int padding = 8;

            Rectangle maus = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 4, 4);

            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Vector2(startX, startY), new Rectangle(0, 0, (buttonSize + spacing) * 5, (buttonSize + spacing) * 5),  Color.Black);

            int curY = 0;
            Vector2 drawPos = Vector2.Zero;
            ButtonType chosen = default;
            for (int i = 0; i < buttons.Count; i++)
            {
                ButtonType b = buttons[i];
                Main.instance.LoadItem(b.itemType);
                Texture2D item = TextureAssets.Item[b.itemType].Value;
                if (i % 5 == 0 && i > 0)
                    curY++;
                float posY = startY + curY * (spacing + buttonSize);

                Rectangle butRect = new Rectangle(startX + buttonSize * (i % 5) + spacing * (i % 5) + padding, (int)posY, buttonSize, buttonSize + padding);
                float itemScale = 0.8f;
                if (maus.Intersects(butRect))
                {
                    itemScale = 0.9f;
                }


                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Vector2(startX + buttonSize * (i % 5) + spacing * (i % 5), posY) + new Vector2(padding), new Rectangle(0, 0, buttonSize, buttonSize), Color.LightBlue, 0, Vector2.Zero, 1, 0, 0);
                spriteBatch.Draw(item, new Vector2(startX + buttonSize * (i % 5) + spacing * (i % 5), posY) + item.Size() / 2 + new Vector2(padding), null, Color.White, 0, item.Size() / 2, new Vector2((float)buttonSize / (float)item.Width, (float)buttonSize / (float)item.Height) * itemScale, 0, 0);

                if (maus.Intersects(butRect))
                {
                    chosen = b;
                    drawPos = butRect.BottomLeft();
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        b.action.Invoke();
                }
            }
            if (drawPos != Vector2.Zero && chosen != default)
            {
                float height  = FontAssets.MouseText.Value.MeasureString(chosen.name).Y + FontAssets.MouseText.Value.MeasureString(chosen.description).Y + 14;
                float width = FontAssets.MouseText.Value.MeasureString(chosen.description).X;
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, drawPos + Vector2.UnitY * 8, new Rectangle(0, 0, (int)width, (int)height), Color.Gray, 0, Vector2.Zero, 1, 0, 0);
                

                Utils.DrawBorderString(spriteBatch, chosen.name, drawPos + Vector2.UnitY * 8, Color.Red);
                Utils.DrawBorderString(spriteBatch, chosen.description, drawPos + FontAssets.MouseText.Value.MeasureString(chosen.name).Y * Vector2.UnitY + Vector2.UnitY * 8, Color.White);
            }
            base.DrawSelf(spriteBatch);
        }
    }

    public class ButtonType
    {
        public string name { get; set; }
        public string description { get; set; }
        public int itemType { get; set; }
        public Action action { get; set; }
        public ButtonType(string name, int itemType, string description, Action action)
        {
            this.name = name;
            this.itemType = itemType;
            this.action = action;
            this.description = description;
            WikiUI.buttons.Add(this);
        }
    }

    [Autoload(Side = ModSide.Client)]
    internal class WikiUISystem : ModSystem
    {
        private UserInterface UserInter;

        internal WikiUI Anoui;

        public static LocalizedText ExampleResourceText { get; private set; }

        public override void Load()
        {
            Anoui = new();
            UserInter = new();
            UserInter.SetState(Anoui);

            LoadButtons();

            string category = "UI";
            ExampleResourceText ??= Mod.GetLocalization($"{category}.ExampleResource");
        }

        public static void LoadButtons()
        {
            new ButtonType("Generate All Item Pages", ItemID.IronBroadsword,
                "Generates individual pages for every single item",
                () => WikiPlayer.ExportPage(WikiPlayer.GenerateAllItemPages));

            new ButtonType("Generate Bestiary Page", ItemID.ZombieArm,
                "Generates a Bestiary database page",
                WikiPlayer.BestiaryPage);

            new ButtonType("Generate Master Template", ItemID.EyeofCthulhuMasterTrophy,
                "Prints a list of every item separated into categories, formatted for master template use",
                () => WikiPlayer.ExportPage(WikiPlayer.GenerateMasterTemplate));

            new ButtonType("Generate Wings Table", ItemID.AngelWings,
                "Generates a table of wing stats",
                () => WikiPlayer.ExportPage(WikiPlayer.GenerateWingStats));

            new ButtonType("Generate Vanity Accessory Table", ItemID.DiamondRing,
                "Generates a table of vanity accessories",
                () => WikiPlayer.ExportPage(WikiPlayer.GenerateVanityAccessories));

            new ButtonType("Generate Vanity Piece Table", ItemID.SunMask,
                "Generates a table of vanity pieces",
                () => WikiPlayer.ExportPage(WikiPlayer.GenerateVanityStandalone));

            new ButtonType("Generate Vanity Set Table", ItemID.BlueLunaticRobe,
                "Generates a table of vanity sets",
                () => WikiPlayer.ExportPage(WikiPlayer.GenerateVanitySets));

            new ButtonType("Generate Consumables Page", ItemID.Mushroom,
                "Generates a page documenting all misc items",
                () => WikiPlayer.ExportPage(WikiPlayer.GenerateConsumablesPage));

            new ButtonType("Generate Blocks Page", ItemID.DirtBlock,
                "Generates a page documenting all blocks (no furniture!)",
                () => WikiPlayer.ExportPage(WikiPlayer.GenerateBlocks));

            new ButtonType("Generate Items Page", ItemID.Torch,
                "Generates a page listing every single item",
                () => WikiPlayer.ExportPage(WikiPlayer.GenerateItemsPageLikeTheBIGOne));

            new ButtonType("Export all block previews", ItemID.BlueBrick,
                "Exports 3x3 previews for every block. Does not account for custom framing.",
                () => WikiPlayer.ExportPage(WikiPlayer.GenerateItemsPageLikeTheBIGOne));

            new ButtonType("Generate recipe subpage", ItemID.TinkerersWorkshop,
                "Generates a page with every recipe that uses the current held item as the station" +
                "\nHold an item that doesn't place any tiles to get \"By hand\" recipes" +
                "\nHold an Ebonstone for Demon Altar recipes",
                () => WikiPlayer.GenerateAllRecipes(Main.LocalPlayer.HeldItem.createTile));

            new ButtonType("Generate NPC Master Template", ItemID.SlimeHook,
                "Generates a list of NPCs sorted by biome/event.",
                () => WikiPlayer.ExportPage(WikiPlayer.GenerateNPCMasterTemplate));

        }

        public override void UpdateUI(GameTime gameTime)
        {
            UserInter?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (resourceBarIndex != -1)
            {
                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "CalValWiki:WikiUI",
                    delegate
                    {
                        UserInter.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}