while true; do
    echo "Which version of Starterkit you want to use: \n\n 1. Bootstrap + SCSS \n 2. Tailwind + PostCSS \n"
    read -p "" yn
    case $yn in
        [1]* ) echo "option 1"; break;;
        [2]* ) echo "option 2"; exit;;
        * ) echo "Please answer 1 or 2.";;
    esac
done