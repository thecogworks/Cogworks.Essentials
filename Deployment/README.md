# Deployments

### **Dev & Staging** - ongoing development and tests

The pipeline picks builds from the standard **env-dev** and **env-staging** branches. To deploy push the desired branch to selected environment branch and await results on the destination server instance.


### **Client Release** - full release supposed to land on PROD environment (e.g. _2.9.11_)

>**IMPORTANT**: Quisque velit nisi, pretium ut lacinia in, elementum id enim. Proin eget tortor risus. Cras ultricies ligula sed magna dictum porta. Vivamus suscipit tortor eget felis porttitor volutpat. Lorem ipsum dolor sit amet, consectetur adipiscing elit.

Follow the general company standards with [GitFlow](../.github/workflows/main.yml) and changelog generated pipeline:

1. Create a **release branch** for the release number (e.g. _release/2.9.11_) and push it to the origin.

2. Perform the adjustments on the branch if needed e.g. last fixes related with the **release**.

3. Add an **annotated tag** with the release number (e.g. _2.9.11_) on the release branch and push tag to the origin.
4. The above step should trigger the [**automation on OUR Github**](https://github.com/thecogworks/cog-project-boilerplate/actions) and regenerate changelog + move the tag to the master branch.

5. After approved work on Client-Staging/UAT environment, **Approve (manually) the deployment** for Live environment and let the pipeline continue deployment to the preproduction slot of the final/LIVE environment.

6. On preprod slots check if **umbraco publish cache** and **Examine indexes** are properly build. If anything is wrong rebuild index or rebuild database and memory cache.

7. When preproduction Live deployment is done you'd have to **Approve it manually** one more time.
