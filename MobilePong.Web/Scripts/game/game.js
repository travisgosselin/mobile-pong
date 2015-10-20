// shim layer with setTimeout fallback
window.requestAnimFrame = (function () {
    return window.requestAnimationFrame ||
            window.webkitRequestAnimationFrame ||
            window.mozRequestAnimationFrame ||
            function (callback) {
                window.setTimeout(callback, 1000 / 60);
            };
})();

function GameCtrl($scope, $window) {
    // initalization of the game control
    $scope.init = function () {
        // prompt to get the username
        $scope.overview = {};
        $scope.overview.Players = [];

        // get singal R off the widnow object
        $scope.signalr = $.connection;
        $scope.game = $scope.signalr.gameHub;
        $scope.canvas = $('#game-area');
        $scope.context = $scope.canvas[0].getContext('2d');
        $scope.isPaddle = false;

        // event that occurs on ball update
        $scope.game.client.StartGame = $scope.onStartGame;
        $scope.game.client.UpdateGame = $scope.onUpdateGame;
        $scope.game.client.Move = $scope.onMove;
        $scope.game.client.Goal = $scope.onGoal;

        // add in key events for moving paddle
        $(document).keydown($scope.onKeyDown);
        $(document).on('swipeleft', $scope.MoveRight);
        $(document).on('swiperight', $scope.MoveLeft);
        $(document).on('tap', $scope.PowerUp);
    };

    $scope.joinGame = function (username, isPaddle) {
        // join the current game
        $scope.username = username;
        $scope.signalr.hub.start({ waitForPageLoad: false }).done(function () {
            var screenWidth = $(window).width() - 50; // 15 px padding on left and right, 20px for scrollbar
            var screenHeight = $(window).height() - 30; // 15 px padding on top and bottom
            $scope.game.server.join($scope.username, screenWidth, screenHeight, isPaddle);
        });
    };

    // renders all components on every animation frame (speed by browser).
    $scope.onRenderAnimationFrame = function () {
        // clear everything to redraw
        $scope.context.clearRect(0, 0, $scope.overview.Settings.GameWidth, $scope.overview.Settings.GameHeight);
		
        // ball/paddle colours to use;
        var colors = [];
        colors.push('red');
        colors.push('yellow');
        colors.push('blue');
        colors.push('green');
        colors.push('orange');

        // animate the balls moving
        var colorIndex = 0;
        angular.forEach($scope.overview.Players, function (player) {
            if (!player.IsPaddleController) {
                $scope.context.beginPath();
                $scope.context.fillStyle = colors[colorIndex];
                $scope.context.arc(player.Ball.X, player.Ball.Y, $scope.overview.Settings.BallRadius, 0, 2 * Math.PI, false);
                $scope.context.fill();
                $scope.context.lineWidth = 1;
                $scope.context.strokeStyle = 'black';
                $scope.context.stroke();
                $scope.context.closePath();
                colorIndex++;
            }
        });

        // animate the paddles moving
        colorIndex = 0;
        angular.forEach($scope.overview.Players, function (player) {
            if (player.IsPaddleController) {
                // calculate paddle
                var paddleY = $scope.overview.Settings.GameHeight - $scope.overview.Settings.PaddleHeight; // this is static only can move x
                var paddleX = player.Paddle.X - (player.Paddle.Width / 2);

                // paddle drawing
                $scope.context.beginPath();
                $scope.context.rect(paddleX, paddleY, player.Paddle.Width, $scope.overview.Settings.PaddleHeight);
                $scope.context.fillStyle = colors[colorIndex];
                $scope.context.fill();
                $scope.context.stroke();
                $scope.context.closePath();
                colorIndex++;
            }
        });

        // ensure we animate next frame on time
        window.requestAnimFrame($scope.onRenderAnimationFrame);
    };

    $scope.onStartGame = function (overview, isPaddle) {
        $scope.$apply(function () {
            $scope.overview = overview;
            $scope.isPaddle = isPaddle;
            $scope.currentPlayer = $scope.getPlayerByName($scope.username);

            $scope.onRenderAnimationFrame();
            $window.location.hash = "game";
        });
    };		

    $scope.onMove = function (update) {
        $scope.$apply(function () {
            // updates the balls for each active player
            angular.forEach(update.Balls, function (ball) {
                var player = $scope.getPlayerByName(ball.Name);
                if (player != null) {
                    player.Ball.X = ball.BallX;
                    player.Ball.Y = ball.BallY;
                }
            });
            
            // updates the paddle for each viewer
            angular.forEach(update.Paddles, function (paddle) {
                var player = $scope.getPlayerByName(paddle.Name);
                if (player != null) {
                    player.Paddle.X = paddle.PaddleX;
                    player.Paddle.Width = paddle.PaddleWidth;
                }
            });
        });
    };

    $scope.onGoal = function (goal) {
        var player = $scope.getPlayerByName(goal.Name);
        if (player != null) {
            $scope.$apply(function() {
                player.Goals = goal.Goals;
                toastr.success("Goal for " + player.Name);
            });
        }
    };

    $scope.onUpdateGame = function (overview) {
        $scope.$apply(function () {
            $scope.overview = overview;
        });
    };

    $scope.onKeyDown = function(event){
        if (event.which == 37) {
            // move left
            $scope.MoveRight();
            event.preventDefault();
        }
        else if (event.which == 39) {
            // move right
            $scope.MoveLeft();
            event.preventDefault();
        }
        else if (event.which == 32) {
            // use powerup such as speed burst or expand
            $scope.PowerUp();
            event.preventDefault();
        }
    };

    $scope.MoveRight = function (event) {
        if ($scope.overview.Status == 1) {
            if ($scope.isPaddle) {
                $scope.currentPlayer.Paddle.X -= $scope.overview.Settings.PaddleSpeed;
                $scope.game.server.movePaddle($scope.currentPlayer.Paddle.X);
            } else {
                $scope.game.server.moveBall(true);
            }
            
            if (event) event.preventDefault();
        }
    };

    $scope.MoveLeft = function (event) {
        if ($scope.overview.Status == 1) {
            if ($scope.isPaddle) {
                $scope.currentPlayer.Paddle.X += $scope.overview.Settings.PaddleSpeed;
                $scope.game.server.movePaddle($scope.currentPlayer.Paddle.X);
            } else {
                $scope.game.server.moveBall(false);
            }
            
            if (event) event.preventDefault();
        }
    };

    $scope.PowerUp = function (event) {
        if ($scope.overview.Status == 1) {
            if (!$scope.isPaddle) {
                $scope.game.server.speedBurst();
                if (event) event.preventDefault();
            } else if ($scope.isPaddle) {
                $scope.game.server.paddleExpand();
                if (event) event.preventDefault();
            }
        }
    };

    $scope.Reset = function() {
        if ($scope.isPaddle) {
            $scope.game.server.reset();
            window.location = '..';
        }
    };

    $scope.getPlayerByName = function(name) {
        var player = null;
        angular.forEach($scope.overview.Players, function(p) {
            if (p.Name == name) {
                player = p;
                return;
            }
        });

        return player;
    };

    // initializes
    $scope.init();
}