﻿using System;
using System.Collections.Generic;

namespace FFImageLoading.Work
{
	public enum ImageSource
	{
		Filepath,
		Url,
		ApplicationBundle
	}

	public class TaskParameter
	{
		/// <summary>
		/// Constructs a new TaskParameter to load an image from a file.
		/// </summary>
		/// <returns>The new TaskParameter.</returns>
		/// <param name="filepath">Path to the file.</param>
		public static TaskParameter FromFile(string filepath)
		{
			return new TaskParameter() { Source = ImageSource.Filepath, Path = filepath };
		}

		/// <summary>
		/// Constructs a new TaskParameter to load an image from a URL.
		/// </summary>
		/// <returns>The new TaskParameter.</returns>
		/// <param name="url">URL to the file</param>
		/// <param name="cacheDuration">How long the file will be cached on disk</param>
		public static TaskParameter FromUrl(string url, TimeSpan? cacheDuration = null)
		{
			return new TaskParameter() { Source = ImageSource.Url, Path = url, CacheDuration = cacheDuration };
		}

		/// <summary>
		/// Constructsa new TaskParameter to load an image from a file from application bundle.
		/// </summary>
		/// <param name="filepath">Path to the file.</param>
		/// <returns>The new TaskParameter.</returns>
		public static TaskParameter FromApplicationBundle(string filepath)
		{
			return new TaskParameter() { Source = ImageSource.ApplicationBundle, Path = filepath };
		}

		private TaskParameter()
		{
            Transformations = new List<ITransformation>();

			// default values so we don't have a null value
			OnSuccess = (w, h) =>
			{
			};

			OnError = ex =>
			{
			};

			OnFinish = scheduledWork =>
			{
			};
		}

		public ImageSource Source { get; private set; }

		public string Path { get; private set; }

		public TimeSpan? CacheDuration { get; private set; }

		public Tuple<int, int> DownSampleSize { get; private set; }

		public ImageSource LoadingPlaceholderSource { get; private set; }

		public string LoadingPlaceholderPath { get; private set; }

		public ImageSource ErrorPlaceholderSource { get; private set; }

		public string ErrorPlaceholderPath { get; private set; }

		public int RetryCount { get; private set; }

		public int RetryDelayInMs { get; private set; }

		public Action<int, int> OnSuccess { get; private set; }

		public Action<Exception> OnError { get; private set; }

		public Action<IScheduledWork> OnFinish { get; private set; }

        public List<ITransformation> Transformations { get; private set; }

		public TaskParameter Transform(ITransformation transformation)
		{
			if (transformation == null)
				throw new NullReferenceException("The transformation argument was null.");

			Transformations.Add(transformation);
			return this;
		}

		public TaskParameter Transform(IEnumerable<ITransformation> transformations)
		{
			if (transformations == null)
				throw new ArgumentNullException("The transformations argument was null");

			Transformations.AddRange(transformations);
			return this;
		}

		/// <summary>
		/// Defines the placeholder used while loading.
		/// </summary>
		/// <param name="path">Path to the file.</param>
		/// <param name="source">Source for the path: local, web, assets</param>
		public TaskParameter LoadingPlaceholder(string path, ImageSource source = ImageSource.Filepath)
		{
			LoadingPlaceholderPath = path;
			LoadingPlaceholderSource = source;
			return this;
		}

		/// <summary>
		/// Defines the placeholder used when an error occurs.
		/// </summary>
		/// <param name="filepath">Path to the file.</param>
		/// <param name="source">Source for the path: local, web, assets</param>
		public TaskParameter ErrorPlaceholder(string filepath, ImageSource source = ImageSource.Filepath)
		{
			ErrorPlaceholderPath = filepath;
			ErrorPlaceholderSource = source;
			return this;
		}

		/// <summary>
		/// Reduce memory usage by downsampling the image. Aspect ratio will be kept even if width/height values are incorrect.
		/// </summary>
		/// <returns>The TaskParameter instance for chaining the call.</returns>
		/// <param name="width">Optional width parameter, if value is higher than zero it will try to downsample to this width while keeping aspect ratio.</param>
		/// <param name="height">Optional height parameter, if value is higher than zero it will try to downsample to this height while keeping aspect ratio.</param>
		public TaskParameter DownSample(int width = 0, int height = 0)
		{
			DownSampleSize = Tuple.Create(width, height);
			return this;
		}

		/// <summary>
		/// If image loading fails automatically retry it a number of times, with a specific delay.
		/// </summary>
		/// <returns>The TaskParameter instance for chaining the call.</returns>
		/// <param name="retryCount">Number of retries</param>
		/// <param name="millisecondDelay">Delay in milliseconds between each trial</param>
		public TaskParameter Retry(int retryCount = 0, int millisecondDelay = 0)
		{
			RetryCount = retryCount;
			RetryDelayInMs = millisecondDelay;
			return this;
		}

		/// <summary>
		/// If image loading succeded this callback is called
		/// </summary>
		/// <returns>The TaskParameter instance for chaining the call.</returns>
		/// <param name="action">Action to invoke when loading succeded.</param>
		public TaskParameter Success(Action action)
		{
			if (action == null)
				throw new Exception("Given lambda should not be null.");

			OnSuccess = (w, h) => action();
			return this;
		}

		/// <summary>
		/// If image loading succeded this callback is called
		/// </summary>
		/// <returns>The TaskParameter instance for chaining the call.</returns>
		/// <param name="action">Action to invoke when loading succeded. Argument is the size of the image loaded.</param>
		public TaskParameter Success(Action<int, int> action)
		{
			if (action == null)
				throw new Exception("Given lambda should not be null.");

			OnSuccess = action;
			return this;
		}

		/// <summary>
		/// If image loading failed this callback is called
		/// </summary>
		/// <returns>The TaskParameter instance for chaining the call.</returns>
		/// <param name="action">Action to invoke when loading failed
		public TaskParameter Error(Action<Exception> action)
		{
			if (action == null)
				throw new Exception("Given lambda should not be null.");

			OnError = action;
			return this;
		}

		/// <summary>
		/// If image loading process finished, whatever the result, this callback is called
		/// </summary>
		/// <returns>The TaskParameter instance for chaining the call.</returns>
		/// <param name="action">Action to invoke when process is done
		public TaskParameter Finish(Action<IScheduledWork> action)
		{
			if (action == null)
				throw new Exception("Given lambda should not be null.");

			OnFinish = action;
			return this;
		}
	}
}

