﻿using Terraria;
using Terraria.ModLoader;
using TerrariaOverhaul.Common.Systems.Gores;
using TerrariaOverhaul.Common.Tags;
using TerrariaOverhaul.Utilities.Extensions;

namespace TerrariaOverhaul.Common.ModEntities.Projectiles
{
	[Autoload(Side = ModSide.Client)]
	public class ProjectileGoreInteraction : GlobalProjectile
	{
		public bool dontHitGore;

		public override bool InstancePerEntity => true;

		public override void AI(Projectile projectile)
		{
			//Reset dontHitGore every X ticks when the projectile's flying somewhere
			if(dontHitGore && projectile.position != projectile.oldPosition && projectile.timeLeft % 3 == 0) {
				dontHitGore = false;
			}

			//Skip gore enumeration when there's nothing to do.
			if(dontHitGore) {
				return;
			}

			bool incendiary = OverhaulProjectileTags.Incendiary.Has(projectile.type);
			bool extinguisher = !incendiary && OverhaulProjectileTags.Extinguisher.Has(projectile.type);

			for(int i = 0; i < Main.maxGore; i++) {
				var gore = Main.gore[i];

				if(gore == null || !gore.active || !(Main.gore[i] is OverhaulGore goreExt)) {
					continue;
				}

				//Intersection check
				if(!projectile.getRect().Intersects(new Microsoft.Xna.Framework.Rectangle((int)gore.position.X, (int)gore.position.Y, (int)goreExt.Width, (int)goreExt.health))) {
					continue;
				}

				/*if(goreExt.position.X >= projectile.position.X + projectile.width
				|| projectile.position.X >= goreExt.position.X + goreExt.Width
				|| goreExt.position.Y >= projectile.position.Y + projectile.height
				|| projectile.position.Y >= goreExt.position.Y + goreExt.Height) {
					continue;
				}*/

				//Interact
				float hitPower = 1f;

				if(incendiary) {
					goreExt.onFire = true;

					continue;
				} else if(extinguisher) {
					goreExt.onFire = false;

					hitPower = 5f;
				}

				dontHitGore = goreExt.HitGore(projectile.velocity.SafeNormalize(default), hitPower);

				break;
			}
		}
	}
}