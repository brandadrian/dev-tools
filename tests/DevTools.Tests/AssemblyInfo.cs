// Console.Out is process-global; tests that redirect it (CaptureConsoleOutput helpers) must not run concurrently.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
