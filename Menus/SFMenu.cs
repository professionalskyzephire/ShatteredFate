using InnoVault;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using System;
using ReLogic.Content;

namespace ShatteredFate.Menus
{
    [VaultLoaden("Menus/")]
    internal class SFMenu : ModMenu
    {
        //纹理资源
        public static Texture2D BackgroundStarless; //无星背景
        public static Texture2D BackgroundStarlessFont; //无星背景的前置遮罩
        public static Asset<Texture2D> Moon;               //月亮
        public static Texture2D Star;               //星星
        [VaultLoaden("@InnoVault/Assets/placeholder2")]
        public static Texture2D Pixel;              //1x1的白色像素纹理，用于绘制流星尾巴和Shader

        //星星管理
        private static List<MenuStar> stars;
        private static bool resourcesInitialized = false;
        private const int STAR_COUNT = 200; //增加星星数量使星空更密集

        //流星管理
        private static ShootingStar shootingStar;
        private static int shootingStarTimer;

        //月亮属性
        internal static float moonScale = 1f;

        public override Asset<Texture2D> MoonTexture => Moon;

        //音乐和背景样式保持不变
        public override int Music => MusicLoader.GetMusicSlot("ShatteredFate/Sounds/Music/best_hl_intro_ever");
        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => SFMenuBack.Instance;

        //初始化所有资源
        private static void InitializeResources() {
            if (resourcesInitialized) {
                return;
            }

            //初始化星星
            stars = new List<MenuStar>();
            for (int i = 0; i < STAR_COUNT; i++) {
                stars.Add(new MenuStar());
            }

            //初始化流星计时器
            ResetShootingStarTimer();

            resourcesInitialized = true;
        }

        private static void ResetShootingStarTimer() {
            //设置一个随机的下次流星生成时间
            shootingStarTimer = Main.rand.Next(60, 120); //1到2秒之间
        }

        public override void OnDeselected() {
            //当菜单关闭时，重置初始化状态，以便下次正确加载
            base.OnDeselected();
            resourcesInitialized = false;
            stars?.Clear();
        }

        public override void Update(bool isOnTitleScreen) {
            //确保所有资源只被初始化一次
            InitializeResources();

            //更新月亮的脉动效果，使用Sin函数使其平滑过渡
            moonScale = 1f + 0.02f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f);

            //更新每一颗星星的状态
            foreach (var star in stars) {
                star.AI();
            }

            //更新流星
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
            //检查纹理是否已加载，防止出错
            if (Moon == null || Star == null || Pixel == null) {
                return true;
            }

            //绘制所有星星
            foreach (var star in stars) {
                star.DrawInUI(spriteBatch);
            }

            Vector2 moonPosition = new Vector2(Main.screenWidth * 0.5f, Main.screenHeight * 0.2f);

            //绘制月亮本体（在光晕之上）
            spriteBatch.Draw(
                Moon.Value,
                moonPosition,
                null,
                Color.White,
                0f,
                Moon.Size() / 2, //将绘制原点设为月亮中心
                moonScale,
                SpriteEffects.None,
                0f
            );

            //绘制流星
            shootingStar?.Draw(spriteBatch);

            Main.spriteBatch.Draw(BackgroundStarlessFont, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);

            return false;
        }
    }

    //星星类
    internal class MenuStar
    {
        public Vector2 Position;
        public float Scale;
        public float Alpha;
        public int Frame;
        private Vector2 velocity;
        private readonly float sineOffset; //用于Sin函数计算透明度


        public MenuStar() {
            //在整个屏幕范围内随机生成位置
            Position = new Vector2(Main.rand.NextFloat(Main.screenWidth), Main.rand.NextFloat(Main.screenHeight / 2) - 20);
            Scale = Main.rand.NextFloat(0.2f, 0.6f);
            Frame = Main.rand.Next(4);
            //根据星星大小赋予不同的移动速度，形成视差效果
            float speed = Scale * 0.05f + 0.02f;
            velocity = new Vector2(speed, speed * 0.1f);
            sineOffset = Main.rand.NextFloat((float)Math.PI * 2); //随机相位，让星星不同步闪烁
        }

        public void AI() {
            Position += velocity;
            //使用Sin函数平滑地更新透明度，制造更自然的闪烁效果
            Alpha = 0.1f + (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 1.5f + sineOffset) + 1) / 2f * 0.7f;

            //屏幕环绕逻辑
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

        public void DrawInUI(SpriteBatch spriteBatch) {
            if (SFMenu.Star == null) {
                return;
            }

            int textureWidth = SFMenu.Star.Width;
            int frameHeight = SFMenu.Star.Height / 4;
            Rectangle sourceRectangle = new Rectangle(0, frameHeight * Frame, textureWidth, frameHeight);
            Vector2 origin = new Vector2(textureWidth / 2f, frameHeight / 2f);

            spriteBatch.Draw(
                SFMenu.Star,
                Position,
                sourceRectangle,
                Color.White * Alpha,
                0f,
                origin,
                Scale,
                SpriteEffects.None,
                0f
            );
        }
    }

    //流星类
    internal class ShootingStar
    {
        private Vector2 position;
        private Vector2 velocity;
        private int life;
        private const int MAX_LIFE = 120; //流星持续时间
        private const int TAIL_LENGTH = 30; //尾巴长度

        public bool IsDone => life <= 0;

        public ShootingStar() {
            //从屏幕外的一个随机点开始
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
            //计算尾巴的起点
            Vector2 tailEnd = position - Vector2.Normalize(velocity) * TAIL_LENGTH * (life / (float)MAX_LIFE);
            float rotation = velocity.ToRotation() + MathHelper.Pi;
            float length = Vector2.Distance(position, tailEnd);
            float alpha = (float)Math.Sin(life / (float)MAX_LIFE * Math.PI); //使用Sin函数实现淡入淡出

            //绘制发光核心
            spriteBatch.Draw(SFMenu.Pixel, position, null, Color.White * alpha, 0f, Vector2.One / 2, 4f, SpriteEffects.None, 0f);

            //绘制尾巴
            spriteBatch.Draw(SFMenu.Pixel,
                position,
                new Rectangle(0, 0, 1, 1),
                new Color(200, 220, 255) * alpha * 0.7f,
                rotation,
                new Vector2(0, 0.5f), //原点在尾巴的起点
                new Vector2(length, 1.5f), //用Scale来拉伸像素点形成线条
                SpriteEffects.None, 0);
        }
    }

    //背景绘制类
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
                Main.spriteBatch.Draw(SFMenu.BackgroundStarless, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
            }
            return false; //阻止原版的背景绘制逻辑
        }
    }
}