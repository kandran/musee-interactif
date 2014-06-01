//------------------------------------------------------------------------------
///Programme de cabine d'essayage intéractive en 3D à partir du logiciel d'avatars fourni avec la kinect
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.Avateering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Kinect;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// Ce programme permet de porter virtuellement des vêtements en 3D
    /// </summary>
    public class AvateeringXNA : Microsoft.Xna.Framework.Game
    {
        #region Fields

        /// <summary>
        /// Permet d'ajuster la taille de la fenêtre. La hauteur est instaurée automatiquement à partir de la largeur en instaurant un ratio 4:3
        /// </summary>
        private const int WindowedWidth = 800;

        /// <summary>
        /// Utilisé  pour ajuster la largeur en plein écran. Seules des résolutions valides peuvent être instaurées.
        /// </summary>
        private const int FullScreenWidth = 1280;

        /// <summary>
        /// Utilisé  pour ajuster la hauteur en plein écran. Seules des résolutions valides peuvent être instaurées.
        /// </summary>
        private const int FullScreenHeight = 1024;

        /// <summary>
        /// Incrément d'arc de Camera 
        /// </summary>
        private const float CameraArcIncrement = 0.1f;

        /// <summary>
        /// Valeur d'angle limite de l'arc de Camera
        /// </summary>
        private const float CameraArcAngleLimit = 90.0f;

        /// <summary>
        /// Valeur d'incrément du zoom de la caméra
        /// </summary>
        private const float CameraZoomIncrement = 0.25f;

        /// <summary>
        /// Valeur de distance max de la caméra
        /// </summary>
        private const float CameraMaxDistance = 500.0f;

        /// <summary>
        /// Valeur de distance min de la caméra
        /// </summary>
        private const float CameraMinDistance = 10.0f;

        /// <summary>
        /// Valeur de distance de départ de la caméra
        /// </summary>
        private const float CameraHeight = 40.0f;

        /// <summary>
        /// Valeur de distance de départ de la caméra
        /// </summary>
        private const float CameraStartingTranslation = 90.0f;

        /// <summary>
        /// Le vêtement en 3D mesure une taille indéfinie en centimètres
        /// Ici on redéfinit la translation de la kinect, ainsi le squelette qui porte les vêtements a les pieds sur le sol
        /// </summary>
        private static readonly Vector3 SkeletonTranslationScaleFactor = new Vector3(40.0f, 40.0f, 40.0f);

        /// <summary>
        /// Le gestionnaire de périphérique graphique fourni par XNA.
        /// </summary>
        private readonly GraphicsDeviceManager graphics;

        /// <summary>
        /// Sélectionne un capteur, et affiche un message si celui-ci n'est pas connecté
        /// </summary>
        private readonly KinectChooser chooser;

        /// <summary>
        /// Gère le rendu du flux de profondeur.
        /// </summary>
        private readonly DepthStreamRenderer depthStream;

        /// <summary>
        /// Génère le rendu du squelette
        /// </summary>
        private readonly SkeletonStreamRenderer skeletonStream;

        /// <summary>
        /// Effet basic de XNA utilisés dans le dessin
        /// </summary>
        private BasicEffect effect;

        /// <summary>
        ///  SpriteBatch utilisé pour l'en-tête/pied de page
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Pour basculer entre le mode fenêtré et plein écran.
        /// </summary>
        private bool fullscreenMode = false;

        /// <summary>
        /// Etat précédent du clavier
        /// </summary>
        private KeyboardState previousKeyboard;

        /// <summary>
        /// Etat actuel du clavier
        /// </summary>
        private KeyboardState currentKeyboard;

        /// <summary>
        /// Texture pour l'en-tête
        /// </summary>
        private Texture2D header;

        /// <summary>
        /// Croix directionnelle utilisée pour dessiner le systèmes de coordonnées (axes)
        /// </summary>
        private CoordinateCross worldAxes;

        /// <summary>
        /// Modèle de vêtement
        /// </summary>
        private Model currentModel;

        /// <summary>
        /// Stocke le mappage entre NuiJoint l'indice d'os du squelette
        /// </summary>
        private Dictionary<JointType, int> nuiJointToAvatarBoneIndex;

        /// <summary>
        /// Animateur du vêtement
        /// </summary>
        private AvatarAnimator animator;

        /// <summary>
        /// Pour voir l'arc de la camera.
        /// </summary>
        private float cameraArc = 0;

        /// <summary>
        /// Pour voir la rotation de la camera
        /// La caméra virtuelle démarre quand la Kinect regarde, cad regarde le long de l'axe Z, avec +X à gauche, Y en haut, Z en avant.
        /// </summary>
        private float cameraRotation = 0;

        /// <summary>
        /// Distance de la camera depuis l'origine
        /// Le vêtement est défini en cm, donc on utilisera le cm.
        /// </summary>
        private float cameraDistance = CameraStartingTranslation;

        /// <summary>
        /// Vue de la matrice.
        /// </summary>
        private Matrix view;

        /// <summary>
        /// Projection de la matrice.
        /// </summary>
        private Matrix projection;

        /// <summary>
        /// Dessine la grille sur laquelle se tient notre squelette
        /// </summary>
        private bool drawGrid;

        /// <summary>
        /// Grille simple est plane de plan XZ.
        /// </summary>
        private GridXz planarXzGrid;

        /// <summary>
        /// Dessine le vêtement uniquement quand le squelette de la personne est détecté.
        /// </summary>
        private bool drawAvatarOnlyWhenPlayerDetected;

        /// <summary>
        /// Flag pour la première détection du squelette.
        /// </summary>
        private bool skeletonDetected;

        /// <summary>
        /// Créé une posture assise quand le mode est activé.
        /// </summary>
        private bool setSeatedPostureInSeatedMode;

        /// <summary>
        /// Fixe la hauteur à partir du centre entre les deux hanches du squelette.
        /// </summary>
        private bool fixAvatarHipCenterDrawHeight;

        /// <summary>
        /// Hauteur ainsi définie.
        /// </summary>
        private float avatarHipCenterDrawHeight;

        /// <summary>
        /// Ajuste l'inclinaison du squelette (quand il se penche en arrière par exemple)
        /// </summary>
        private bool leanAdjust;

        ///<summary>
        ///Pour le flux d'images
        ///</summary>
        private readonly ColorStreamRenderer colorStream;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes une nouvelle instance de la classe AvateeringXNA.
        /// </summary>
        public AvateeringXNA()
        {
            this.Window.Title = "Avateering";
            this.IsFixedTimeStep = false;
            this.IsMouseVisible = true;

            // Configure le périphérique graphique pour le rendu
            this.graphics = new GraphicsDeviceManager(this);
            this.SetScreenMode();
            this.graphics.PreparingDeviceSettings += this.GraphicsDevicePreparingDeviceSettings;
            this.graphics.SynchronizeWithVerticalRetrace = true;

            Content.RootDirectory = "Content";

            //Le capteur de la kinect utilisera 640*480 pour le flux de couleur (par défaut) et 320*240 pour la profondeur
            this.chooser = new KinectChooser(this, ColorImageFormat.RgbResolution640x480Fps30, DepthImageFormat.Resolution320x240Fps30);
            this.Services.AddService(typeof(KinectChooser), this.chooser);

            // Met Optionellement le mode de près pour vue de près (de 0.4m à 3m)
            this.chooser.NearMode = false;

            // Pour Optionnellement régler le mode assis pour le haut du corps seulement (généralement utilisé avec le mode de près de la caméra)
            this.chooser.SeatedMode = false;

            // Ajouter ces objets comme des composants de jeux XNA établit des appels automatiques aux méthodes LoadContent, Update, etc.. 
            this.Components.Add(this.chooser);

            // Créele plan sur lequel se tient le modèle.
            this.planarXzGrid = new GridXz(this, new Vector3(0, 0, 0), new Vector2(500, 500), new Vector2(10, 10), Color.Black);
            this.Components.Add(this.planarXzGrid);
            this.drawGrid = true;

            this.worldAxes = new CoordinateCross(this, 500);
            this.Components.Add(this.worldAxes);

            // Crée l'animateur du modèle.
            this.animator = new AvatarAnimator(this, this.RetargetMatrixHierarchyToAvatarMesh, AvateeringXNA.SkeletonTranslationScaleFactor);
            this.Components.Add(this.animator);

            // Options de dessin.
            this.setSeatedPostureInSeatedMode = true;
            this.drawAvatarOnlyWhenPlayerDetected = true;
            this.skeletonDetected = false;
            this.leanAdjust = true;

            // Ici, on peut forcer l'avatar à être dessiné à une hauteur fixe dans le monde virtuel XNA.
            // La raison pour laquelle on utilise ceci est que la hauteur par rapport au plancher n'est pas toujours connue,
            // et si l'avatar n'est pas correctement placé, il ne fera que sautiller.
            this.fixAvatarHipCenterDrawHeight = true;
            this.avatarHipCenterDrawHeight = 0.8f;  // in meters

            // Met en place le flux de profondeur.
            this.depthStream = new DepthStreamRenderer(this);

            //Pour la couleur
            this.colorStream = new ColorStreamRenderer(this);

            // Met en place le flux du squelette.
            this.skeletonStream = new SkeletonStreamRenderer(this, this.SkeletonToDepthMap);

            //Met à jour la profondeur, la taille du squelette et la location
            this.UpdateStreamSizeAndLocation();

            this.previousKeyboard = Keyboard.GetState();
        }

        /// <summary>
        /// Obtient le KinectChooser.
        /// </summary>
        public KinectChooser Chooser
        {
            get
            {
                return (KinectChooser)this.Services.GetService(typeof(KinectChooser));
            }
        }

        /// <summary>
        /// Obtient le SpriteBatch.
        /// </summary>
        public SpriteBatch SharedSpriteBatch
        {
            get
            {
                return (SpriteBatch)this.Services.GetService(typeof(SpriteBatch));
            }
        }

        /// <summary>
        /// Obtient ou crée les données du squelette.
        /// </summary>
        private static Skeleton[] SkeletonData { get; set; }

        /// <summary>
        /// Charge le contenu graphique.
        /// </summary>
        protected override void LoadContent()
        {
            // Crée le spritebatch pour dessiner les éléments 3D.
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.Services.AddService(typeof(SpriteBatch), this.spriteBatch);

            this.header = Content.Load<Texture2D>("Header");

            // Crée les effets basic de XNA pour le dessin de ligne.
            this.effect = new BasicEffect(GraphicsDevice);
            if (null == this.effect)
            {
                throw new InvalidOperationException("Cannot load Basic Effect");
            }

            this.effect.VertexColorEnabled = true;

            // Charge le modèle.
            this.currentModel = Content.Load<Model>("Vetement");
            if (null == this.currentModel)
            {
                throw new InvalidOperationException("Cannot load 3D avatar model");
            }

            // Ajoute le modèle à l'animateur.
            this.animator.Avatar = this.currentModel;
            this.animator.AvatarHipCenterHeight = this.avatarHipCenterDrawHeight;

            // Met le Nui joint en modélisation de modèle pour ce modèle.
            this.BuildJointHierarchy();

            base.LoadContent();
        }

        /// <summary>
        /// Cette fonction fait l'équivalence entre le squelette de la kinect et le squelette du modèle
        /// </summary>
        protected void BuildJointHierarchy()
        {
            // "Vetement.fbx" Indices d'os.
            // Voici la liste des os que la transformation affecte. (En anglais).
            //Les valeurs de rotation sont stockées au niveau du joint avant le début de l'os 
            //(c'est à dire au niveau du joint commun avec l'extrémité de l'os de la mère).
            // 0 = root node
            // 1 = pelvis
            // 2 = spine
            // 3 = spine1
            // 4 = spine2
            // 5 = spine3
            // 6 = neck
            // 7 = head
            // 8-11 = eyes
            // 12 = Left clavicle (joint between spine and shoulder)
            // 13 = Left upper arm (joint at left shoulder)
            // 14 = Left forearm
            // 15 = Left hand
            // 16-30 = Left hand finger bones
            // 31 = Right clavicle (joint between spine and shoulder)
            // 32 = Right upper arm (joint at left shoulder)
            // 33 = Right forearm
            // 34 = Right hand
            // 35-49 = Right hand finger bones
            // 50 = Left Thigh
            // 51 = Left Knee
            // 52 = Left Ankle
            // 53 = Left Ball
            // 54 = Right Thigh
            // 55 = Right Knee
            // 56 = Right Ankle
            // 57 = Right Ball

            // Pour le Kinect NuiSkeleton, l'articulation à la fin de l'os décrit la rotation pour y arriver, 
            // et l'articulation racine est dans HipCenter. C'est différent du squelette de l'avatar décrit ci-dessus
            if (null == this.nuiJointToAvatarBoneIndex)
            {
                this.nuiJointToAvatarBoneIndex = new Dictionary<JointType, int>();
            }
            //Note: l'articulation du centre de la hanche dans le modèle 3D a un noeud racine (index 0), qu'on ignore ici pour la rotation
            this.nuiJointToAvatarBoneIndex.Add(JointType.HipCenter, 1);
            this.nuiJointToAvatarBoneIndex.Add(JointType.Spine, 4);
            this.nuiJointToAvatarBoneIndex.Add(JointType.ShoulderCenter, 6);
            this.nuiJointToAvatarBoneIndex.Add(JointType.Head, 7);
            this.nuiJointToAvatarBoneIndex.Add(JointType.ElbowLeft, 13);
            this.nuiJointToAvatarBoneIndex.Add(JointType.WristLeft, 14);
            this.nuiJointToAvatarBoneIndex.Add(JointType.HandLeft, 15);
            this.nuiJointToAvatarBoneIndex.Add(JointType.ElbowRight, 32);
            this.nuiJointToAvatarBoneIndex.Add(JointType.WristRight, 33);
            this.nuiJointToAvatarBoneIndex.Add(JointType.HandRight, 34);
            this.nuiJointToAvatarBoneIndex.Add(JointType.KneeLeft, 50);
            this.nuiJointToAvatarBoneIndex.Add(JointType.AnkleLeft, 51);
            this.nuiJointToAvatarBoneIndex.Add(JointType.FootLeft, 52);
            this.nuiJointToAvatarBoneIndex.Add(JointType.KneeRight, 54);
            this.nuiJointToAvatarBoneIndex.Add(JointType.AnkleRight, 55);
            this.nuiJointToAvatarBoneIndex.Add(JointType.FootRight, 56);
        }

        #endregion

        #region Update

        /// <summary>
        /// Permet au jeu de fonctionner.
        /// </summary>
        /// <param name="gameTime">The gametime.</param>
        protected override void Update(GameTime gameTime)
        {
            // Met à jour l'état sauvegardé
            this.previousKeyboard = this.currentKeyboard;

            //Si le capteur n'est pas trouvé, ne tourne pas ou n'est pas connecté, stop
            if (null == this.chooser || null == this.Chooser.Sensor || false == this.Chooser.Sensor.IsRunning || this.Chooser.Sensor.Status != KinectStatus.Connected)
            {
                return;
            }

            bool newFrame = false;

            using (var skeletonFrame = this.Chooser.Sensor.SkeletonStream.OpenNextFrame(0))
            {
                //Des fois on obtient une trame null si aucune donnée n'est prête
                if (null != skeletonFrame)
                {
                    newFrame = true;

                    // Réaffecter si nécessaire
                    if (null == SkeletonData || SkeletonData.Length != skeletonFrame.SkeletonArrayLength)
                    {
                        SkeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }

                    skeletonFrame.CopySkeletonDataTo(SkeletonData);

                    // Sélectionne le premier squelette suivi que nous allons "avatariser"
                    Skeleton rawSkeleton =
                        (from s in SkeletonData
                         where s != null && s.TrackingState == SkeletonTrackingState.Tracked
                         select s).FirstOrDefault();

                    if (null != this.animator)
                    {
                        if (null != rawSkeleton)
                        {
                            this.animator.CopySkeleton(rawSkeleton);
                            this.animator.FloorClipPlane = skeletonFrame.FloorClipPlane;

                            //Reset les filtres si le squelette n'a pas été vu avant maintenant
                            if (this.skeletonDetected == false)
                            {
                                this.animator.Reset();
                            }

                            this.skeletonDetected = true;
                            this.animator.SkeletonVisible = true;
                        }
                        else
                        {
                            this.skeletonDetected = false;
                            this.animator.SkeletonVisible = false;
                        }
                    }
                }
            }

            if (newFrame)
            {
                //Appelle la mise à jour de flux manuellement car ce ne sont pas des composants de "game"
                if (null != this.depthStream && null != this.skeletonStream && null != this.colorStream)
                {
                    this.depthStream.Update(gameTime);
                    this.colorStream.Update(gameTime);
                    this.skeletonStream.Update(gameTime, SkeletonData);
                }

                // Met à jour le rendu de l'avatar
                if (null != this.animator)
                {
                    this.animator.SkeletonDrawn = false;
                }
            }

            this.HandleInput();
            this.UpdateCamera(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// Crée la camera
        /// </summary>
        protected void UpdateViewingCamera()
        {
            GraphicsDevice device = this.graphics.GraphicsDevice;

            // Calcule les matrices de la caméra
            this.view = Matrix.CreateTranslation(0, -CameraHeight, 0) *
                          Matrix.CreateRotationY(MathHelper.ToRadians(this.cameraRotation)) *
                          Matrix.CreateRotationX(MathHelper.ToRadians(this.cameraArc)) *
                          Matrix.CreateLookAt(
                                                new Vector3(0, 0, -this.cameraDistance),
                                                new Vector3(0, 0, 0),
                                                Vector3.Up);

            // Kinect vertical FOV en degrés
            float nominalVerticalFieldOfView = 45.6f;

            if (null != this.chooser && null != this.Chooser.Sensor && this.Chooser.Sensor.IsRunning && KinectStatus.Connected == this.Chooser.Sensor.Status)
            {
                nominalVerticalFieldOfView = this.chooser.Sensor.DepthStream.NominalVerticalFieldOfView;
            }

            this.projection = Matrix.CreatePerspectiveFieldOfView(
                                                                nominalVerticalFieldOfView * (float)Math.PI / 180.0f,
                                                                device.Viewport.AspectRatio,
                                                                1,
                                                                10000);
        }

        #endregion

        #region Draw

        /// <summary>
        /// Cette méthode renvoit l'état en cours
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>

        protected override void Draw(GameTime gameTime)
        {
            // Efface l'écran en noir
            GraphicsDevice.Clear(Color.Black);

            this.UpdateViewingCamera();

            // Affiche le flux de couleur et le squelette (on peut également afficher le flux de profondeur)
            if (null != this.depthStream && null != this.skeletonStream && null != this.colorStream)
            {
                this.colorStream.Draw(gameTime);
                //this.depthStream.Draw(gameTime);
                this.skeletonStream.Draw(gameTime);
            }

            //Dessine la grille et les axes sur lequel se tient notre avatar
            //Pour nos axes, X=rouge, Y=vert, Z=bleu
            if (this.drawGrid && null != this.planarXzGrid && null != this.worldAxes)
            {
                this.planarXzGrid.Draw(gameTime, Matrix.Identity, this.view, this.projection);
                this.worldAxes.Draw(gameTime, Matrix.Identity, this.view, this.projection);
            }

            // Dessine l'avatar
            if (null != this.animator && (!this.drawAvatarOnlyWhenPlayerDetected || (this.drawAvatarOnlyWhenPlayerDetected && this.skeletonDetected)))
            {
                this.animator.Draw(gameTime, Matrix.Identity, this.view, this.projection);
            }

            // Affiche les images d'en-tête et pied de page
            this.SharedSpriteBatch.Begin();
            this.SharedSpriteBatch.Draw(this.header, Vector2.Zero, null, Color.White);
            this.SharedSpriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        #region Handle Input

        /// <summary>
        /// Commandes pour quitter le jeu
        /// </summary>
        private void HandleInput()
        {
            this.currentKeyboard = Keyboard.GetState();

            // Pour quitter: touche echap
            if (this.currentKeyboard.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            // Plein écran avec F11
            if (this.currentKeyboard.IsKeyDown(Keys.F11))
            {
                // Si ce n'était pas le cas lors de la dernière mise à jour, la touche vient d'être appuyée.
                if (!this.previousKeyboard.IsKeyDown(Keys.F11))
                {
                    this.fullscreenMode = !this.fullscreenMode;
                    this.SetScreenMode();
                }
            }

            // Dessiner l'avatar lorsqu'il n'est pas détecté: on/off
            if (this.currentKeyboard.IsKeyDown(Keys.V))
            {
                if (!this.previousKeyboard.IsKeyDown(Keys.V))
                {
                    this.drawAvatarOnlyWhenPlayerDetected = !this.drawAvatarOnlyWhenPlayerDetected;
                }
            }

            // Modes près/assis: on/off
            if (this.currentKeyboard.IsKeyDown(Keys.N))
            {
                if (!this.previousKeyboard.IsKeyDown(Keys.N))
                {
                    this.chooser.SeatedMode = !this.chooser.SeatedMode;
                    this.skeletonDetected = false;

                    // Set near mode to accompany seated mode
                    this.chooser.NearMode = this.chooser.SeatedMode;
                }
            }

            //Fixer le centre des hances de l'avatar lors du dessin (pour estimer la hauteur)
            if (this.currentKeyboard.IsKeyDown(Keys.H))
            {
                if (!this.previousKeyboard.IsKeyDown(Keys.H))
                {
                    this.fixAvatarHipCenterDrawHeight = !this.fixAvatarHipCenterDrawHeight;
                }
            }

            // Fixer les penchements en avant/arrière de l'avatar
            if (this.currentKeyboard.IsKeyDown(Keys.L))
            {
                if (!this.previousKeyboard.IsKeyDown(Keys.L))
                {
                    this.leanAdjust = !this.leanAdjust;
                }
            }

            // Reset les filtres de l'avatar (rester également la caméra)
            if (this.currentKeyboard.IsKeyDown(Keys.R))
            {
                if (!this.previousKeyboard.IsKeyDown(Keys.R))
                {
                    if (null != this.animator)
                    {
                        this.animator.Reset();
                    }
                }
            }
        }

        /// <summary>
        /// Basculer entre le mode plein écran et fenêtré
        /// </summary>
        private void SetScreenMode()
        {
            //met la résolution de l'écran ou la taille de la fenêtre à la taille désirée
            //Si fenêtré, oblige aussi un ratio de 4:3 pour la taille et ajoute 110 pour les en-tête / pied de page
            if (this.fullscreenMode)
            {
                foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    //Compare nos hauteur/largeurs en plein écran avec chaque mode d'affichage pour définir leur validité
                    if ((mode.Width == FullScreenWidth) && (mode.Height == FullScreenHeight))
                    {
                        this.graphics.PreferredBackBufferWidth = FullScreenWidth;
                        this.graphics.PreferredBackBufferHeight = FullScreenHeight;
                        this.graphics.IsFullScreen = true;
                        this.graphics.ApplyChanges();
                    }
                }
            }
            else
            {
                if (WindowedWidth <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                {
                    this.graphics.PreferredBackBufferWidth = WindowedWidth;
                    this.graphics.PreferredBackBufferHeight = ((WindowedWidth / 4) * 3) + 110;
                    this.graphics.IsFullScreen = false;
                    this.graphics.ApplyChanges();
                }
            }

            this.UpdateStreamSizeAndLocation();
        }

        /// <summary>
        /// Met à jour les flux de profondeur et la position/taille du squelette en fonction de la résolution
        /// </summary>
        private void UpdateStreamSizeAndLocation()
        {
            int depthStreamWidth = this.graphics.PreferredBackBufferWidth / 1;
            Vector2 size = new Vector2(depthStreamWidth, (depthStreamWidth / 4) * 3);
            Vector2 pos = new Vector2(this.graphics.PreferredBackBufferWidth - depthStreamWidth, 85);

            if (null != this.depthStream)
            {
                this.depthStream.Size = size;
                this.depthStream.Position = pos;
            }

            if (null != this.skeletonStream)
            {
                this.skeletonStream.Size = size;
                this.skeletonStream.Position = pos;
            }

            if (null != this.colorStream)
            {
                this.colorStream.Size = size;
                this.colorStream.Position = pos;
            }
        }

        /// <summary>
        /// Traite les données de la caméra
        /// </summary>
        /// <param name="gameTime">The gametime.</param>
        private void UpdateCamera(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            //Regarde les données pour tourner la caméra en haut et en bas autour du modèle
            if (this.currentKeyboard.IsKeyDown(Keys.Up) ||
                this.currentKeyboard.IsKeyDown(Keys.W))
            {
                this.cameraArc += time * CameraArcIncrement;
            }

            if (this.currentKeyboard.IsKeyDown(Keys.Down) ||
                this.currentKeyboard.IsKeyDown(Keys.S))
            {
                this.cameraArc -= time * CameraArcIncrement;
            }

            // Limite les mouvements d'angle
            if (this.cameraArc > CameraArcAngleLimit)
            {
                this.cameraArc = CameraArcAngleLimit;
            }
            else if (this.cameraArc < -CameraArcAngleLimit)
            {
                this.cameraArc = -CameraArcAngleLimit;
            }

            // Regarde les donner pour tourner la caméra autour du modèle
            if (this.currentKeyboard.IsKeyDown(Keys.Right) ||
                this.currentKeyboard.IsKeyDown(Keys.D))
            {
                this.cameraRotation += time * CameraArcIncrement;
            }

            if (this.currentKeyboard.IsKeyDown(Keys.Left) ||
                this.currentKeyboard.IsKeyDown(Keys.A))
            {
                this.cameraRotation -= time * CameraArcIncrement;
            }

            // Regarde les données pour zoomer/dézoomer
            if (this.currentKeyboard.IsKeyDown(Keys.Z))
            {
                this.cameraDistance += time * CameraZoomIncrement;
            }

            if (this.currentKeyboard.IsKeyDown(Keys.X))
            {
                this.cameraDistance -= time * CameraZoomIncrement;
            }

            // Limite la distance entre la caméra et l'origine
            if (this.cameraDistance > CameraMaxDistance)
            {
                this.cameraDistance = CameraMaxDistance;
            }
            else if (this.cameraDistance < CameraMinDistance)
            {
                this.cameraDistance = CameraMinDistance;
            }

            if (this.currentKeyboard.IsKeyDown(Keys.R))
            {
                this.cameraArc = 0;
                this.cameraRotation = 0;
                this.cameraDistance = CameraStartingTranslation;
            }
        }

        #endregion

        #region AvatarRetargeting

        /// <summary>
        /// Les Modèles d'avatar 3D/de vêtements... ont généralement des structures osseuses et des orientations d'articulations qui varient, selon la façon dont ils sont construits.
        /// Ici, nous adaptons les matrices de rotation de façon à ce qu'elles fonctionnent avec notre avatar et on met celles-ci dans le tableau
        /// BoneTransforms. Ce tableau est ensuite converti plus tard en "world transform" et les "skinning transform" envoyées au XNA skinning processor
        /// pour dessiner le maillage.
        /// Le modèle "Vâtement.fbx" définit plus d'os/articulations (57 au total) et dans des endroits et orientations différentes du Nui Skeleton (squelette de la Kinect)
        /// De nombreux os/articulations n'ont pas d'équivalents directs, par exemple les doigts que la kinect n'est pas assez précise pour gérer. 
        /// Les os sont définis parents les uns par rapport aux autres, et les os inconnus sont laissés comme identités relatives.
        /// Une transformation de la matrice boneTransforms les obligent à prendre l'orientation de leur parent dans le monde de coordonnées XNA.
        ///  3D avatar models typically have varying bone structures and joint orientations, depending on how they are built.
        /// </summary>
        /// <param name="skeleton">The Kinect skeleton.</param>
        /// <param name="bindRoot">The bind root matrix of the avatar mesh.</param>
        /// <param name="boneTransforms">The avatar mesh rotation matrices.</param>
        private void RetargetMatrixHierarchyToAvatarMesh(Skeleton skeleton, Matrix bindRoot, Matrix[] boneTransforms)
        {
            if (null == skeleton)
            {
                return;
            }

            //Etablit les données de rotation des os dans l'armature de l'avatar
            foreach (BoneOrientation bone in skeleton.BoneOrientations)
            {
                // Si des os ne sont pas reconnus, les ignorer
                // Notons que si on fait marcher des filtres sur les données brutes du squelette,
                // Qui fixe les problèmes de tracking, on devra faire passer l'état de tracking de "NotTracket" à "Inferred"
                if (skeleton.Joints[bone.EndJoint].TrackingState == JointTrackingState.NotTracked)
                {
                    continue;
                }

                this.SetJointTransformation(bone, skeleton, bindRoot, ref boneTransforms);
            }

            //Si le mode "seated" (assis) est on, assoir l'avatar
            if (this.Chooser.SeatedMode && this.setSeatedPostureInSeatedMode)
            {
                this.SetSeatedPosture(ref boneTransforms);
            }

            // Etablit la position de l'avatar dans le "monde"
            this.SetAvatarRootWorldPosition(skeleton, ref boneTransforms);
        }

        /// <summary>
        /// Etablit la transformation des os dans l'armature 3D de l'avatar
        /// </summary>
        /// <param name="bone">Nui Joint/bone orientation</param>
        /// <param name="skeleton">The Kinect skeleton.</param>
        /// <param name="bindRoot">The bind root matrix of the avatar mesh.</param>
        /// <param name="boneTransforms">The avatar mesh rotation matrices.</param>
        private void SetJointTransformation(BoneOrientation bone, Skeleton skeleton, Matrix bindRoot, ref Matrix[] boneTransforms)
        {
            // Toujours regarder la racine du squelette
            if (bone.StartJoint == JointType.HipCenter && bone.EndJoint == JointType.HipCenter)
            {
                // Sauf si c'est le mode assis, le centre des hanches est spécial - c'est la racine du NuiSkeleton et ça décrit l'orientation du squelette dans le monde
                // (camera) système de coordonnées. Toutes les autres orientations des os/armatures dans la hierarchie ont le centre des hanches comme un de leurs parents.
                // Cependant, si on est en mode assis, c'est le centre des épaules qui sert de repère à l'orientation du squelette dans le système de coordonnées de la caméra.
                bindRoot.Translation = Vector3.Zero;
                Matrix invBindRoot = Matrix.Invert(bindRoot);

                //Pour ne pas avoir une déformation étrage du modèle 3D, matrice de rotation autour de l'axe Z
                Matrix zaxisflip = Matrix.CreateRotationZ(MathHelper.ToRadians(-90));
                invBindRoot *= zaxisflip;
                Matrix hipOrientation = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                // Here we create a rotation matrix for the hips from the inverse of the bind pose
                // for the pelvis rotation and the inverse of the bind pose for the root node (0) in the "Vêtements" model.
                // This multiplication effectively removes the initial 90 degree rotations set in the first two model nodes.
                Matrix pelvis = boneTransforms[1];
                pelvis.Translation = Vector3.Zero; // Ensure pure rotation as we explicitly set world translation from the Kinect camera below.
                Matrix invPelvis = Matrix.Invert(pelvis);

                Matrix combined = (invBindRoot * hipOrientation) * invPelvis;

                this.ReplaceBoneMatrix(JointType.HipCenter, combined, true, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.ShoulderCenter)
            {
                // This contains an absolute rotation if we are in seated mode, or the hip center is not tracked, as the HipCenter will be identity
                if (this.chooser.SeatedMode || (this.Chooser.SeatedMode == false && skeleton.Joints[JointType.HipCenter].TrackingState == JointTrackingState.NotTracked))
                {
                    bindRoot.Translation = Vector3.Zero;
                    Matrix invBindRoot = Matrix.Invert(bindRoot);

                    Matrix hipOrientation = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                    // We can use the same method as in HipCenter above to invert the root and pelvis bind pose,
                    // however, alternately we can also explicitly swap axes and adjust the rotations to get from
                    // the Kinect rotation to the model hip orientation, similar to what we do for the following joints/bones.

                    // Kinect = +X left, +Y up, +Z forward in body coordinate system
                    // Avatar = +Z left, +X up, +Y forward
                    Quaternion kinectRotation = KinectHelper.DecomposeMatRot(hipOrientation);    // XYZ
                    Quaternion avatarRotation = new Quaternion(kinectRotation.Y, kinectRotation.Z, kinectRotation.X, kinectRotation.W); // transform from Kinect to avatar coordinate system
                    Matrix combined = Matrix.CreateFromQuaternion(avatarRotation);

                    // Add a small adjustment rotation to manually correct for the rotation in the parent bind
                    // pose node in the model mesh - this can be found by looking in the FBX or in 3DSMax/Maya.
                    Matrix adjustment = Matrix.CreateRotationY(MathHelper.ToRadians(-90));
                    combined *= adjustment;
                    Matrix adjustment2 = Matrix.CreateRotationZ(MathHelper.ToRadians(-90));
                    combined *= adjustment2;

                    // Although not strictly correct, we apply this to the hip center, as all other bones are children of this joint.
                    // Application at the spine or shoulder center instead would require manually updating of the bone orientations below for the whole body to move when the shoulders twist or tilt.
                    this.ReplaceBoneMatrix(JointType.HipCenter, combined, true, ref boneTransforms);
                }
            }
            else if (bone.EndJoint == JointType.Spine)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                // The Dude appears to lean back too far compared to a real person, so here we adjust this lean.
                this.CorrectBackwardsLean(skeleton, ref tempMat);

                // Also add a small constant adjustment rotation to correct for the hip center to spine bone being at a rear-tilted angle in the Kinect skeleton.
                // The dude should now look more straight ahead when avateering
                Matrix adjustment = Matrix.CreateRotationX(MathHelper.ToRadians(20));  // 20 degree rotation around the local Kinect x axis for the spine bone.
                tempMat *= adjustment;

                // Kinect = +X left, +Y up, +Z forward in body coordinate system
                // Avatar = +Z left, +X up, +Y forward
                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat);    // XYZ
                Quaternion avatarRotation = new Quaternion(kinectRotation.Y, kinectRotation.Z, kinectRotation.X, kinectRotation.W); // transform from Kinect to avatar coordinate system
                tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                // Set the corresponding matrix in the avatar using the translation table we specified.
                // Note for the spine and shoulder center rotations, we could also try to spread the angle
                // over all the Avatar skeleton spine joints, causing a more curved back, rather than apply
                // it all to one joint, as we do here.
                this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.Head)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                // Add a small adjustment rotation to correct for the avatar skeleton head bones being defined pointing looking slightly down, not vertical.
                // The dude should now look more straight ahead when avateering
                Matrix adjustment = Matrix.CreateRotationX(MathHelper.ToRadians(-30));  // -30 degree rotation around the local Kinect x axis for the head bone.
                tempMat *= adjustment;

                // Kinect = +X left, +Y up, +Z forward in body coordinate system
                // Avatar = +Z left, +X up, +Y forward
                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat);    // XYZ
                Quaternion avatarRotation = new Quaternion(kinectRotation.Y, kinectRotation.Z, kinectRotation.X, kinectRotation.W); // transform from Kinect to avatar coordinate system
                tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                // Set the corresponding matrix in the avatar using the translation table we specified
                this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.ElbowLeft || bone.EndJoint == JointType.WristLeft)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                if (bone.EndJoint == JointType.ElbowLeft)
                {
                    // Add a small adjustment rotation to correct for the avatar skeleton shoulder/upper arm bones.
                    // The dude should now be able to have arms correctly down at his sides when avateering
                    Matrix adjustment = Matrix.CreateRotationZ(MathHelper.ToRadians(-15));  // -15 degree rotation around the local Kinect z axis for the upper arm bone.
                    tempMat *= adjustment;
                }

                // Kinect = +Y along arm, +X down, +Z forward in body coordinate system
                // Avatar = +X along arm, +Y down, +Z backwards
                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat);    // XYZ
                Quaternion avatarRotation = new Quaternion(kinectRotation.Y, -kinectRotation.Z, -kinectRotation.X, kinectRotation.W); // transform from Kinect to avatar coordinate system
                tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.HandLeft)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                // Add a small adjustment rotation to correct for the avatar skeleton wist/hand bone.
                // The dude should now have the palm of his hands toward his body when arms are straight down
                Matrix adjustment = Matrix.CreateRotationY(MathHelper.ToRadians(-90));  // -90 degree rotation around the local Kinect y axis for the wrist-hand bone.
                tempMat *= adjustment;

                // Kinect = +Y along arm, +X down, +Z forward in body coordinate system
                // Avatar = +X along arm, +Y down, +Z backwards
                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat);    // XYZ
                Quaternion avatarRotation = new Quaternion(kinectRotation.Y, kinectRotation.X, -kinectRotation.Z, kinectRotation.W);
                tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.ElbowRight || bone.EndJoint == JointType.WristRight)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                if (bone.EndJoint == JointType.ElbowRight)
                {
                    // Add a small adjustment rotation to correct for the avatar skeleton shoulder/upper arm bones.
                    // The dude should now be able to have arms correctly down at his sides when avateering
                    Matrix adjustment = Matrix.CreateRotationZ(MathHelper.ToRadians(15));  // 15 degree rotation around the local Kinect  z axis for the upper arm bone.
                    tempMat *= adjustment;
                }

                // Kinect = +Y along arm, +X up, +Z forward in body coordinate system
                // Avatar = +X along arm, +Y back, +Z down
                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat);    // XYZ
                Quaternion avatarRotation = new Quaternion(kinectRotation.Y, -kinectRotation.Z, -kinectRotation.X, kinectRotation.W); // transform from Kinect to avatar coordinate system
                tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.HandRight)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                // Add a small adjustment rotation to correct for the avatar skeleton wist/hand bone.
                // The dude should now have the palm of his hands toward his body when arms are straight down
                Matrix adjustment = Matrix.CreateRotationY(MathHelper.ToRadians(90));  // -90 degree rotation around the local Kinect y axis for the wrist-hand bone.
                tempMat *= adjustment;

                // Kinect = +Y along arm, +X up, +Z forward in body coordinate system
                // Avatar = +X along arm, +Y down, +Z forwards
                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat);    // XYZ
                Quaternion avatarRotation = new Quaternion(kinectRotation.Y, -kinectRotation.X, kinectRotation.Z, kinectRotation.W); // transform from Kinect to avatar coordinate system
                tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.KneeLeft)
            {
                // Combine the two joint rotations from the hip and knee
                Matrix hipLeft = KinectHelper.Matrix4ToXNAMatrix(skeleton.BoneOrientations[JointType.HipLeft].HierarchicalRotation.Matrix);
                Matrix kneeLeft = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);
                Matrix combined = kneeLeft * hipLeft;

                this.SetLegMatrix(bone.EndJoint, combined, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.AnkleLeft || bone.EndJoint == JointType.AnkleRight)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);
                this.SetLegMatrix(bone.EndJoint, tempMat, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.KneeRight)
            {
                // Combine the two joint rotations from the hip and knee
                Matrix hipRight = KinectHelper.Matrix4ToXNAMatrix(skeleton.BoneOrientations[JointType.HipRight].HierarchicalRotation.Matrix);
                Matrix kneeRight = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);
                Matrix combined = kneeRight * hipRight;

                this.SetLegMatrix(bone.EndJoint, combined, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.FootLeft || bone.EndJoint == JointType.FootRight)
            {
                // Only set this if we actually have a good track on this and the parent
                if (skeleton.Joints[bone.EndJoint].TrackingState == JointTrackingState.Tracked && skeleton.Joints[skeleton.BoneOrientations[bone.EndJoint].StartJoint].TrackingState == JointTrackingState.Tracked)
                {
                    Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                    // Add a small adjustment rotation to correct for the avatar skeleton foot bones being defined pointing down at 45 degrees, not horizontal
                    Matrix adjustment = Matrix.CreateRotationX(MathHelper.ToRadians(-45));
                    tempMat *= adjustment;

                    // Kinect = +Y along foot (fwd), +Z up, +X right in body coordinate system
                    // Avatar = +X along foot (fwd), +Y up, +Z right
                    Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat); // XYZ
                    Quaternion avatarRotation = new Quaternion(kinectRotation.Y, kinectRotation.Z, kinectRotation.X, kinectRotation.W); // transform from Kinect to avatar coordinate system
                    tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                    this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
                }
            }
        }

        /// <summary>
        /// Correct the spine rotation when leaning back to reduce lean.
        /// </summary>
        /// <param name="skeleton">The Kinect skeleton.</param>
        /// <param name="spineMat">The spine orientation.</param>
        private void CorrectBackwardsLean(Skeleton skeleton, ref Matrix spineMat)
        {
            Matrix hipOrientation = KinectHelper.Matrix4ToXNAMatrix(skeleton.BoneOrientations[JointType.HipCenter].HierarchicalRotation.Matrix);

            Vector3 hipZ = new Vector3(hipOrientation.M31, hipOrientation.M32, hipOrientation.M33);   // Z (forward) vector
            Vector3 boneY = new Vector3(spineMat.M21, spineMat.M22, spineMat.M23);   // Y (up) vector

            hipZ *= -1;
            hipZ.Normalize();
            boneY.Normalize();

            // Dot product the hip center forward vector with our spine bone up vector.
            float cosAngle = Vector3.Dot(hipZ, boneY);

            // If it's negative (i.e. greater than 90), we are leaning back, so reduce this lean.
            if (cosAngle < 0 && this.leanAdjust)
            {
                float angle = (float)Math.Acos(cosAngle);
                float correction = (angle / 2) * -(cosAngle / 2);
                Matrix leanAdjustment = Matrix.CreateRotationX(correction);  // reduce the lean by up to half, scaled by how far back we are leaning
                spineMat *= leanAdjustment;
            }
        }

        /// <summary>
        /// Helper used for leg bones.
        /// </summary>
        /// <param name="joint">Nui Joint index</param>
        /// <param name="legRotation">Matrix containing a leg joint rotation.</param>
        /// <param name="boneTransforms">The avatar mesh rotation matrices.</param>
        private void SetLegMatrix(JointType joint, Matrix legRotation, ref Matrix[] boneTransforms)
        {
            // Kinect = +Y along leg (down), +Z fwd, +X right in body coordinate system
            // Avatar = +X along leg (down), +Y fwd, +Z right
            Quaternion kinectRotation = KinectHelper.DecomposeMatRot(legRotation);  // XYZ
            Quaternion avatarRotation = new Quaternion(kinectRotation.Y, kinectRotation.Z, kinectRotation.X, kinectRotation.W); // transform from Kinect to avatar coordinate system
            legRotation = Matrix.CreateFromQuaternion(avatarRotation);

            this.ReplaceBoneMatrix(joint, legRotation, false, ref boneTransforms);
        }

        /// <summary>
        /// Set the avatar root position in world coordinates.
        /// </summary>
        /// <param name="skeleton">The Kinect skeleton.</param>
        /// <param name="boneTransforms">The avatar mesh rotation matrices.</param>
        private void SetAvatarRootWorldPosition(Skeleton skeleton, ref Matrix[] boneTransforms)
        {
            // Get XNA world position of skeleton.
            Matrix worldTransform = this.GetModelWorldTranslation(skeleton.Joints, this.chooser.SeatedMode);

            // set root translation
            boneTransforms[0].Translation = worldTransform.Translation;
        }

        /// <summary>
        /// This function sets the mapping between the Nui Skeleton bones/joints and the Avatar bones/joints
        /// </summary>
        /// <param name="joint">Nui Joint index</param>
        /// <param name="boneMatrix">Matrix to set in joint/bone.</param>
        /// <param name="replaceTranslationInExistingBoneMatrix">set Boolean true to replace the translation in the original bone matrix with the one passed in boneMatrix (i.e. at root), false keeps the original (default).</param>
        /// <param name="boneTransforms">The avatar mesh rotation matrices.</param>
        private void ReplaceBoneMatrix(JointType joint, Matrix boneMatrix, bool replaceTranslationInExistingBoneMatrix, ref Matrix[] boneTransforms)
        {
            int meshJointId;
            bool success = this.nuiJointToAvatarBoneIndex.TryGetValue(joint, out meshJointId);

            if (success)
            {
                Vector3 offsetTranslation = boneTransforms[meshJointId].Translation;
                boneTransforms[meshJointId] = boneMatrix;

                if (replaceTranslationInExistingBoneMatrix == false)
                {
                    // overwrite any new boneMatrix translation with the original one
                    boneTransforms[meshJointId].Translation = offsetTranslation;   // re-set the translation
                }
            }
        }

        /// <summary>
        /// Helper used to get the world translation for the root.
        /// </summary>
        /// <param name="joints">Nui Joint collection.</param>
        /// <param name="seatedMode">Boolean true if seated mode.</param>
        /// <returns>Returns a Matrix containing the translation.</returns>
        private Matrix GetModelWorldTranslation(JointCollection joints, bool seatedMode)
        {
            Vector3 transVec = Vector3.Zero;

            if (seatedMode && joints[JointType.ShoulderCenter].TrackingState != JointTrackingState.NotTracked)
            {
                transVec = KinectHelper.SkeletonPointToVector3(joints[JointType.ShoulderCenter].Position);
            }
            else
            {
                if (joints[JointType.HipCenter].TrackingState != JointTrackingState.NotTracked)
                {
                    transVec = KinectHelper.SkeletonPointToVector3(joints[JointType.HipCenter].Position);
                }
                else if (joints[JointType.ShoulderCenter].TrackingState != JointTrackingState.NotTracked)
                {
                    // finally try shoulder center if this is tracked while hip center is not
                    transVec = KinectHelper.SkeletonPointToVector3(joints[JointType.ShoulderCenter].Position);
                }
            }

            if (this.fixAvatarHipCenterDrawHeight)
            {
                transVec.Y = this.avatarHipCenterDrawHeight;
            }

            // Here we scale the translation, as the "Dude" avatar mesh is defined in centimeters, and the Kinect skeleton joint positions in meters.
            return Matrix.CreateTranslation(transVec * SkeletonTranslationScaleFactor);
        }

        /// <summary>
        /// Sets the Avatar in a seated posture - useful for seated mode.
        /// </summary>
        /// <param name="boneTransforms">The relative bone transforms of the avatar mesh.</param>
        private void SetSeatedPosture(ref Matrix[] boneTransforms)
        {
            // In the Kinect coordinate system, we first rotate from the local avatar 
            // root orientation with +Y up to +Y down for the leg bones (180 around Z)
            // then pull the knees up for a seated posture.
            Matrix rot180 = Matrix.CreateRotationZ(MathHelper.ToRadians(180));
            Matrix rot90 = Matrix.CreateRotationX(MathHelper.ToRadians(90));
            Matrix rotMinus90 = Matrix.CreateRotationX(MathHelper.ToRadians(-90));
            Matrix combinedHipRotation = rot90 * rot180;

            this.SetLegMatrix(JointType.KneeLeft, combinedHipRotation, ref boneTransforms);
            this.SetLegMatrix(JointType.KneeRight, combinedHipRotation, ref boneTransforms);
            this.SetLegMatrix(JointType.AnkleLeft, rotMinus90, ref boneTransforms);
            this.SetLegMatrix(JointType.AnkleRight, rotMinus90, ref boneTransforms);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// This method ensures that we can render to the back buffer without
        /// losing the data we already had in our previous back buffer.  This
        /// is necessary for the SkeletonStreamRenderer.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event args.</param>
        private void GraphicsDevicePreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            // This is necessary because we are rendering to back buffer/render targets and we need to preserve the data
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }

        /// <summary>
        /// This method maps a SkeletonPoint to the depth frame.
        /// </summary>
        /// <param name="point">The SkeletonPoint to map.</param>
        /// <returns>A Vector2 of the location on the depth frame.</returns>
        private Vector2 SkeletonToDepthMap(SkeletonPoint point)
        {
            // This is used to map a skeleton point to the depth image location
            if (null == this.chooser || null == this.Chooser.Sensor || true != this.Chooser.Sensor.IsRunning || this.Chooser.Sensor.Status != KinectStatus.Connected)
            {
                return Vector2.Zero;
            }

            var depthPt = this.chooser.Sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(point, this.chooser.Sensor.DepthStream.Format);

            // scale to current depth image display size and add any position offset
            float x = (depthPt.X * this.skeletonStream.Size.X) / this.chooser.Sensor.DepthStream.FrameWidth;
            float y = (depthPt.Y * this.skeletonStream.Size.Y) / this.chooser.Sensor.DepthStream.FrameHeight;

            return new Vector2(x + this.skeletonStream.Position.X, y + this.skeletonStream.Position.Y);
        }

        #endregion
    }
}