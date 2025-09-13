using InnoVault;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace ShatteredFate.Menus
{
    [VaultLoaden("Menus")]
    internal class SFMenu : ModMenu
    {
        //Texture Resources
        public static Texture2D BackgroundStarless;//Starless background
        public static Texture2D BackgroundStarlessFont;//Front mask for the starless background
        public static Asset<Texture2D> Moon;//Moon
        public static Asset<Texture2D> LogoAsset;
        public static Texture2D Star;//Stars
        [VaultLoaden("@InnoVault/Assets/placeholder2")]
        public static Texture2D Pixel;//1x1 white pixel texture, used for drawing shooting star tails and for shaders

        internal static float Sengs = 0f;

        //Star Management
        private static List<MenuStar> stars;
        private static bool resourcesInitialized = false;
        private const int STAR_COUNT = 200;//Increase the number of stars to make the sky denser

        //Shooting Star Management
        private static ShootingStar shootingStar;
        private static int shootingStarTimer;

        //Moon Properties
        internal static float moonScale = 1f;

        public override Asset<Texture2D> MoonTexture => Moon;

        public override Asset<Texture2D> Logo => LogoAsset;

        //Music and background style remain unchanged
        public override int Music => MusicLoader.GetMusicSlot("ShatteredFate/Sounds/Music/best_hl_intro_ever");
        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => SFMenuBack.Instance;

        //Initialize all resources
        private static void InitializeResources() {
            if (resourcesInitialized) {
                return;
            }

            Sengs = 0f;

            //Initialize stars
            stars = new List<MenuStar>();
            for (int i = 0; i < STAR_COUNT; i++) {
                var star = new MenuStar();
                star.SetProperty();
                stars.Add(star);
            }

            //Initialize shooting star timer
            ResetShootingStarTimer();

            resourcesInitialized = true;
        }

        private static void ResetShootingStarTimer() {
            //Set a random time for the next shooting star to appear
            shootingStarTimer = Main.rand.Next(6, 120);
        }

        public override void OnDeselected() {
            //When the menu is closed, reset the initialization state for a correct reload next time
            base.OnDeselected();
            Sengs = 0f;
            resourcesInitialized = false;
            stars?.Clear();
        }

        public override void Update(bool isOnTitleScreen) {
            //Ensure all resources are initialized only once
            InitializeResources();

            if (Sengs < 1f) {
                Sengs += 0.02f;
            }

            //Update the moon's pulsing effect, using a Sine function for a smooth transition
            moonScale = 0.8f + 0.012f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f);

            //Update the state of each star
            foreach (var star in stars) {
                star.AI();
            }

            //Update the shooting star
            if (shootingStar != null) {
                shootingStar.Update();
                if (shootingStar.IsDone) {
                    shootingStar = null;
                }
            }
            else {
                shootingStarTimer--;
                if (shootingStarTimer <= 0) {
                    shootingStar = new ShootingStar();
                    ResetShootingStarTimer();
                }
            }
        }

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor) {
            //Check if textures are loaded to prevent errors
            if (Moon == null || Star == null || Pixel == null) {
                return true;
            }

            //Draw all stars
            foreach (var star in stars) {
                star.DrawInUI(spriteBatch);
            }

            Vector2 moonPosition = new Vector2(Main.screenWidth * 0.5f, Main.screenHeight * 0.2f);

            //Draw the moon itself (above the glow)
            spriteBatch.Draw(
                Moon.Value,
                moonPosition,
                null,
                Color.White * Sengs,
                0f,
                Moon.Size() / 2, //Set the draw origin to the center of the moon
                moonScale,
                SpriteEffects.None,
                0f
            );

            //Draw the shooting star
            shootingStar?.Draw(spriteBatch);

            Main.spriteBatch.Draw(BackgroundStarlessFont, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * Sengs);

            drawColor *= Sengs;
            logoDrawCenter.Y += Main.screenHeight / 12;

            return true;
        }
    }

    internal class MenuStar : BasePRT
    {
        public override string Texture => "ShatteredFate/Menus/Star";

        public override void SetProperty() {
            PRTLayersMode = PRTLayersModeEnum.None;
            //Generate a random position within the top half of the screen
            Position = new Vector2(Main.rand.NextFloat(Main.screenWidth), Main.rand.NextFloat(Main.screenHeight / 2));
            Scale = Main.rand.NextFloat(0.2f, 0.6f);
            Frame = TexValue.GetRectangle(Main.rand.Next(4), 4);
            //Assign different movement speeds based on star size to create a parallax effect
            float speed = Scale * 0.05f + 0.02f;
            Velocity = new Vector2(speed, speed * 0.1f);
            ai[0] = Main.rand.NextFloat((float)Math.PI * 2);//Random phase to make stars twinkle asynchronously
        }

        public override void AI() {
            Position += Velocity;
            //Use a Sine function to smoothly update opacity, creating a more natural twinkling effect
            Opacity = 0.1f + (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 1.5f + ai[0]) + 1) / 2f * 0.7f;

            //Screen wrapping logic
            if (Position.X > Main.screenWidth + 50) {
                Position.X = -50;
            }
            if (Position.X < -50) {
                Position.X = Main.screenWidth + 50;
            }
            if (Position.Y > Main.screenHeight + 50) {
                Position.Y = -50;
            }
            if (Position.Y < -50) {
                Position.Y = Main.screenHeight + 50;
            }
        }

        public override void DrawInUI(SpriteBatch spriteBatch) {
            if (SFMenu.Star == null) {
                return;
            }

            spriteBatch.Draw(
                SFMenu.Star,
                Position,
                Frame,
                Color.White * Opacity * SFMenu.Sengs,
                0f,
                Frame.Size() / 2,
                Scale,
                SpriteEffects.None,
                0f
            );
        }
    }

    //Shooting Star Class
    internal class ShootingStar
    {
        private Vector2 position;
        private Vector2 velocity;
        private int life;
        private const int MAX_LIFE = 120;//Duration of the shooting star
        private const int TAIL_LENGTH = 30;//Tail length

        public bool IsDone => life <= 0;

        public ShootingStar() {
            //Start from a random point outside the screen
            if (Main.rand.NextBool()) {
                position = new Vector2(-50, Main.rand.NextFloat(Main.screenHeight * 0.6f));
                velocity = new Vector2(Main.rand.NextFloat(8f, 15f), Main.rand.NextFloat(2f, 5f));
            }
            else {
                position = new Vector2(Main.screenWidth + 50, Main.rand.NextFloat(Main.screenHeight * 0.6f));
                velocity = new Vector2(Main.rand.NextFloat(-15f, -8f), Main.rand.NextFloat(2f, 5f));
            }
            life = MAX_LIFE;
        }

        public void Update() {
            position += velocity;
            life--;
        }

        public void Draw(SpriteBatch spriteBatch) {
            if (SFMenu.Pixel == null) {
                return;
            }
            //Calculate the starting point of the tail
            Vector2 tailEnd = position - Vector2.Normalize(velocity) * TAIL_LENGTH * (life / (float)MAX_LIFE);
            float rotation = velocity.ToRotation() + MathHelper.Pi;
            float length = Vector2.Distance(position, tailEnd);
            float alpha = (float)Math.Sin(life / (float)MAX_LIFE * Math.PI);//Use a Sine function to achieve a fade-in and fade-out effect

            //Draw the glowing core
            spriteBatch.Draw(SFMenu.Pixel, position, null, Color.White * alpha, 0f, Vector2.One / 2, 4f, SpriteEffects.None, 0f);

            //Draw the tail
            spriteBatch.Draw(SFMenu.Pixel,
                position,
                new Rectangle(0, 0, 1, 1),
                new Color(200, 220, 255) * alpha * 0.7f * SFMenu.Sengs,
                rotation,
                new Vector2(0, 0.5f),//The origin is at the start of the tail
                new Vector2(length, 1.5f),//Use Scale to stretch the pixel into a line
                SpriteEffects.None, 0);
        }
    }

    //Background Drawing Class
    internal class SFMenuBack : ModSurfaceBackgroundStyle
    {
        internal static SFMenuBack Instance { get; private set; }
        public override void Load() => Instance = this;
        public override void Unload() => Instance = null;

        public override void ModifyFarFades(float[] fades, float transitionSpeed) {
            for (int i = 0; i < fades.Length; i++) {
                fades[i] = 0.2f;
            }
        }

        public override bool PreDrawCloseBackground(SpriteBatch spriteBatch) {
            if (SFMenu.BackgroundStarless != null) {
                Main.spriteBatch.Draw(SFMenu.BackgroundStarless, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * SFMenu.Sengs);
            }
            return false;//Prevent the original background drawing logic from running
        }
    }
}