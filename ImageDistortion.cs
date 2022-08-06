using System.Collections.Generic;
using OpenCvSharp;

/// <summary>
/// Image distortion component. input camera matrix and dist coefficients 
/// </summary>
public class ImageDistortion
{
    /// <summary>
    /// Distort image, insert texture, camera matrix and distortion coefficients 
    /// </summary>
    /// <param name="texture2D">Texture to distort</param>
    /// <param name="cameraMatrix">Camera matrix</param>
    /// <param name="distCoefficients">Distortion coefficients of camera</param>
    /// <param name="size">Size of image</param>
    public Texture2D DistortImage(Texture2D texture2D, Mat cameraMatrix, Mat distCoefficients, Size size)
    {
        //Convert texture to mat
        var image_ud = OpenCvSharp.Unity.TextureToMat(texture2D);
        
        //Assign values
        var cam_mtx = cameraMatrix;
        var dis_cef = distCoefficients;
        var image_size = size;

        //Create local variables
        var R = new Mat();
        var cam_mtx_ud = cam_mtx.Clone();
        var map_x = new Mat(image_size, MatType.CV_32FC1);
        var map_y = new Mat(image_size, MatType.CV_32FC1);
        var pts_ud = new List<Point2f>();
        var pts_distort = new List<Point2f>();

        //Get points to list
        for (int y = 0; y < image_size.Height; y++)
        {
            for (int x = 0; x < image_size.Width; x++)
            {
                pts_distort.Insert(pts_distort.Count, (new Point2f(x, y)));
            }
        }

        //Un-distort points
        Cv2.UndistortPoints(InputArray.Create(pts_distort), OutputArray.Create(pts_ud), cam_mtx,
            dis_cef, R, cam_mtx_ud);

        //Re-order points
        for (int y = 0; y < image_size.Height; ++y)
        {
            for (int x = 0; x < image_size.Width; ++x)
            {
                var pt = pts_ud[y * image_size.Width + x];
                map_x.Set(y, x, pt.X);
                map_y.Set(y, x, pt.Y);
            }
        }

        //remap
        var image_distort = new Mat();
        Cv2.Remap(image_ud, image_distort, map_x, map_y, InterpolationFlags.Lanczos4, BorderTypes.Transparent);
        
        //Convert Mat to texture
        var image_distort_texture = OpenCvSharp.Unity.MatToTexture(image_distort);
        return image_distort_texture;
    }
}
