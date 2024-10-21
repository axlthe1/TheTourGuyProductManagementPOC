 podman build . -t product-search-api -f .\TheTourGuy.ProductSearcherApi\Dockerfile
 podman build . -t worker-otherguy -f .\SomeOtherGuyWorker\Dockerfile
 podman build . -t worker-bigguy -f .\TheBigGuyWorker\Dockerfile