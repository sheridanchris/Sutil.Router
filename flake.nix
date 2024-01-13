{
  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/master";
    flake-utils.url = "github:numtide/flake-utils";
    pre-commit-hooks.url = "github:cachix/pre-commit-hooks.nix";
  };
  outputs = {
    self,
    nixpkgs,
    flake-utils,
    pre-commit-hooks,
    ...
  }:
    flake-utils.lib.eachDefaultSystem (system: let
      pkgs = import nixpkgs {inherit system;};
    in
      with pkgs; {
        checks = {
          pre-commit-check = pre-commit-hooks.lib.${system}.run {
            src = ./.;
            hooks = {
              fantomas = {
                enable = true;
                name = "fantomas";
                description = "Format your F# code with fantomas.";
                entry = "dotnet fantomas";
                files = "(\\.fs$)|(\\.fsx$)";
              };
            };
          };
        };
        devShells.default = mkShell {
          inherit (self.checks.${system}.pre-commit-check) shellHook;
          packages = [
            dotnet-sdk_8
            nodejs_21
          ];
          DOTNET_ROOT = "${dotnet-sdk_8}";
        };
      });
}
