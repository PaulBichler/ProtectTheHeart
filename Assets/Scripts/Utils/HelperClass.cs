using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

public static class HelperClass
{
    public static async void Await(this Task task)
    {
        await task;
    }

    public static void SetLayerRecursive(this Transform transform, int layer)
    {
        foreach (Transform child in transform.transform)
            child.SetLayerRecursive(layer);

        transform.gameObject.layer = layer;
    }

    public static string GetBulletColors(int layer)
    {
        string filePath = layer switch {
            6 => "Prefabs/Projectiles/Purple",
            7 => "Prefabs/Projectiles/Orange",
            8 => "Prefabs/Projectiles/Blue",
            9 => "Prefabs/Projectiles/Green",
            10 => "Prefabs/Projectiles/Purple",
            11 => "Prefabs/Projectiles/Orange",
            12 => "Prefabs/Projectiles/Blue",
            13 => "Prefabs/Projectiles/Green",
            _ => string.Empty
            };

        return filePath;
    }

    public static GameObject GetBulletHit(int layer)
    {
        string filePath = layer switch {
            6 => "Prefabs/Projectiles/PurpleHit",
            7 => "Prefabs/Projectiles/OrangeHit",
            8 => "Prefabs/Projectiles/BlueHit",
            9 => "Prefabs/Projectiles/GreenHit",
            10 => "Prefabs/Projectiles/PurpleHit",
            11 => "Prefabs/Projectiles/OrangeHit",
            12 => "Prefabs/Projectiles/BlueHit",
            13 => "Prefabs/Projectiles/GreenHit",
            _ => string.Empty
            };

        return Resources.Load<GameObject>(filePath);
    }

    public static GameObject GetBulletExplosion(int layer)
    {
        string filePath = layer switch {
            6 => "Prefabs/Explosions/Explosion_Purple",
            7 => "Prefabs/Explosions/Explosion_Orange",
            8 => "Prefabs/Explosions/Explosion_Blue",
            9 => "Prefabs/Explosions/Explosion_Green",
            _ => string.Empty
            };

        return Resources.Load<GameObject>(filePath);
    }

    public static string GetMuzzleColors(int layer)
    {
        string filePath = layer switch
        {
            6 => "Prefabs/Projectiles/Muzzles/Muzzle_Purple",
            7 => "Prefabs/Projectiles/Muzzles/Muzzle_Orange",
            8 => "Prefabs/Projectiles/Muzzles/Muzzle_Blue",
            9 => "Prefabs/Projectiles/Muzzles/Muzzle_Green",
            _ => string.Empty
        };

        return filePath;
    }

    public static string GetHitSplatterColors(int layer)
    {
        string filePath = layer switch
        {
            6 => "Prefabs/Projectiles/HitSplatter/HitSplatter_Purple",
            7 => "Prefabs/Projectiles/HitSplatter/HitSplatter_Orange",
            8 => "Prefabs/Projectiles/HitSplatter/HitSplatter_Blue",
            9 => "Prefabs/Projectiles/HitSplatter/HitSplatter_Green",
            10 => "Prefabs/Projectiles/HitSplatter/HitSplatter_Purple", 
            11 => "Prefabs/Projectiles/HitSplatter/HitSplatter_Orange",
            12 => "Prefabs/Projectiles/HitSplatter/HitSplatter_Blue",
            13 => "Prefabs/Projectiles/HitSplatter/HitSplatter_Green",
            _ => string.Empty
        };

        return filePath;
    }

    public static string SplitCamelCase( this string str )
    {
        return Regex.Replace( 
            Regex.Replace( 
                str, 
                @"(\P{Ll})(\P{Ll}\p{Ll})", 
                "$1 $2" 
            ), 
            @"(\p{Ll})(\P{Ll})", 
            "$1 $2" 
        );
    }
}