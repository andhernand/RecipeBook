set -e

mongosh <<EOF
use $RECIPE_BOOK_API_DATABASE

db.createUser({
  user: '$RECIPE_BOOK_API_USER',
  pwd: '$RECIPE_BOOK_API_PWD',
  roles: [{
    role: 'readWrite',
    db: '$RECIPE_BOOK_API_DATABASE'
  }]
});

db.createCollection('$RECIPE_BOOK_API_COLLECTION');
EOF
