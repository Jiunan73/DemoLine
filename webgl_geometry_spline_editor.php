﻿<!DOCTYPE html>
<?php
	include('DB_Login.php');
	include('Data_Function.php');
	include('DB_Function_base.php');
?>
<html lang="en">
	<head>
		<title>three.js webgl - geometry - catmull spline editor</title>
		<meta charset="utf-8">
		<meta name="viewport" content="width=device-width, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0">
		<link type="text/css" rel="stylesheet" href="main.css">
		<style>
			body {
				background-color: #f0f0f0;
				color: #444;
			}
			a {
				color: #08f;
			}
		</style>
	</head>
	<body>

		<div id="container"></div>
		<div id="info">
			<a href="https://threejs.org" target="_blank" rel="noopener">three.js</a> - geometry - catmull spline editor
		</div>

		<script type="module">

			import * as THREE from '../build/three.module.js';

			import Stats from './jsm/libs/stats.module.js';
			import { GUI } from './jsm/libs/dat.gui.module.js';

			import { OrbitControls } from './jsm/controls/OrbitControls.js';
			import { TransformControls } from './jsm/controls/TransformControls.js';

			let container, stats;
			let camera, scene, renderer;
			const splineHelperObjects = [];
			let splinePointsLength = 4;
			const positions = [];
			const point = new THREE.Vector3();

			const raycaster = new THREE.Raycaster();
			const pointer = new THREE.Vector2();
			const onUpPosition = new THREE.Vector2();
			const onDownPosition = new THREE.Vector2();
			//-------------
			
			var direction = new THREE.Vector3(1, 0, 0); // 箭头的指向向量
			console.log(direction);
			var origin = new THREE.Vector3(0, 0, 0); // 箭头的起点位置向量
			//direction.normalize(); // 将方向向量归一化
			var axis = new THREE.Vector3(0,1, 0); // 旋转轴，这里使用 Z 轴
			// 创建箭头对象
			var length = 20;
			var arrowHelper=[];
			var horizontalAngle =0; 
			var angle = 45; // 角度值
			var radian = THREE.MathUtils.degToRad(angle); // 将角度转换为弧度

			// 使用三角函数计算向量的 X 和 Y 分量
			var x = Math.cos(radian);
			var y = Math.sin(radian);

			//var arrowHelper = new THREE.ArrowHelper(direction, origin, length, 0xff0000); // 参数依次为：向量方向、起点、箭头长度、箭头颜色
			<?php
			$Query="SELECT `Curr_time`, `tagid`, `X`-800, `Y`-1000, `Z`, WindAngle+155-`Z`, `windspeed`*1000, `windW`*1000, `Status` FROM `wind` where Curr_time between '2023-06-27 09:00:00' and '2023-06-27 09:15:00' and Status in (0) ";
			$Result=MySQL_UTF8_Function($Local_Host, $Local_User, $Local_Password,'agv',"SELECT",$Query);
			for ($i=1;$i<count($Result);$i++)
			{
				$th=0;
				$th=atan2($Result[$i][7],$Result[$i][6])/pi()*180;
				echo "origin.x=".$Result[$i][2].";";
				echo "origin.z=".$Result[$i][3].";";
				//echo "horizontalAngle = THREE.MathUtils.degToRad(".$Result[$i][5].");"; 
				echo "length=".$Result[$i][6].";";
				echo "angle = ".$Result[$i][5].";";
				echo "radian = THREE.MathUtils.degToRad(angle);";
				echo "x = Math.cos(radian)*".$Result[$i][6].";";
				echo "y = Math.sin(radian)*".$Result[$i][6].";";
				
				echo "direction.x=x;";
				
				echo "direction.z=y;";
				echo "direction.y=".$Result[$i][7].";";
				
				//echo "direction.z=y;";
				
				echo "direction.normalize();";
				echo "console.log(direction);";
				echo "console.log(".($Result[$i][7]/$Result[$i][6]).");";
				echo "console.log(angle);";
				echo "console.log(".$th.");";
				//echo "direction.applyAxisAngle(axis, horizontalAngle);";
				echo "arrowHelper[".$i."] = new THREE.ArrowHelper(direction, origin, length, 0xff0000);";
			}
			?>
			
			//---------------------
				
			const geometry = new THREE.BoxGeometry( 20, 20, 20 );
			let transformControl;

			const ARC_SEGMENTS = 200;

			const splines = {};

			const params = {
				uniform: true,
				tension: 0.5,
				centripetal: true,
				chordal: true,
				addPoint: addPoint,
				removePoint: removePoint,
				exportSpline: exportSpline
			};

			init();
			animate();

			function init() {

				container = document.getElementById( 'container' );

				scene = new THREE.Scene();
				scene.background = new THREE.Color( 0xf0f0f0 );

				camera = new THREE.PerspectiveCamera( 70, window.innerWidth / window.innerHeight, 1, 10000 );
				camera.position.set( 0, 250, 1000 );
				scene.add( camera );

				scene.add( new THREE.AmbientLight( 0xf0f0f0 ) );
				const light = new THREE.SpotLight( 0xffffff, 1.5 );
				light.position.set( 0, 1500, 200 );
				light.angle = Math.PI * 0.2;
				light.castShadow = true;
				light.shadow.camera.near = 200;
				light.shadow.camera.far = 2000;
				light.shadow.bias = - 0.000222;
				light.shadow.mapSize.width = 1024;
				light.shadow.mapSize.height = 1024;
				scene.add( light );

				const planeGeometry = new THREE.PlaneGeometry( 2000, 2000 );
				planeGeometry.rotateX( - Math.PI / 2 );
				const planeMaterial = new THREE.ShadowMaterial( { opacity: 0.2 } );

				const plane = new THREE.Mesh( planeGeometry, planeMaterial );
				plane.position.y = - 200;
				plane.receiveShadow = true;
				scene.add( plane );

				const helper = new THREE.GridHelper( 2000, 100 );
				helper.position.y = - 199;
				helper.material.opacity = 0.25;
				helper.material.transparent = true;
				scene.add( helper );
					
				//-----------------------	
				<?php
				for ($i=1;$i<count($Result);$i++)
				{
					echo "scene.add(arrowHelper[".$i."]);";
				}
					
				?>
				
				//------------------

				renderer = new THREE.WebGLRenderer( { antialias: true } );
				renderer.setPixelRatio( window.devicePixelRatio );
				renderer.setSize( window.innerWidth, window.innerHeight );
				renderer.shadowMap.enabled = true;
				container.appendChild( renderer.domElement );

				stats = new Stats();
				container.appendChild( stats.dom );

				const gui = new GUI();

				gui.add( params, 'uniform' );
				gui.add( params, 'tension', 0, 1 ).step( 0.01 ).onChange( function ( value ) {

					splines.uniform.tension = value;
					updateSplineOutline();

				} );
				gui.add( params, 'centripetal' );
				gui.add( params, 'chordal' );
				gui.add( params, 'addPoint' );
				gui.add( params, 'removePoint' );
				gui.add( params, 'exportSpline' );
				gui.open();

				// Controls
				const controls = new OrbitControls( camera, renderer.domElement );
				controls.damping = 0.2;
				controls.addEventListener( 'change', render );

				transformControl = new TransformControls( camera, renderer.domElement );
				transformControl.addEventListener( 'change', render );
				transformControl.addEventListener( 'dragging-changed', function ( event ) {

					controls.enabled = ! event.value;

				} );
				scene.add( transformControl );

				transformControl.addEventListener( 'objectChange', function () {

					updateSplineOutline();

				} );

				document.addEventListener( 'pointerdown', onPointerDown );
				document.addEventListener( 'pointerup', onPointerUp );
				document.addEventListener( 'pointermove', onPointerMove );

				/*******
				 * Curves
				 *********/

				for ( let i = 0; i < splinePointsLength; i ++ ) {

					addSplineObject( positions[ i ] );

				}

				positions.length = 0;

				for ( let i = 0; i < splinePointsLength; i ++ ) {

					positions.push( splineHelperObjects[ i ].position );

				}

				const geometry = new THREE.BufferGeometry();
				geometry.setAttribute( 'position', new THREE.BufferAttribute( new Float32Array( ARC_SEGMENTS * 3 ), 3 ) );

				let curve = new THREE.CatmullRomCurve3( positions );
				curve.curveType = 'catmullrom';
				curve.mesh = new THREE.Line( geometry.clone(), new THREE.LineBasicMaterial( {
					color: 0xff0000,
					opacity: 0.35
				} ) );
				curve.mesh.castShadow = true;
				splines.uniform = curve;

				curve = new THREE.CatmullRomCurve3( positions );
				curve.curveType = 'centripetal';
				curve.mesh = new THREE.Line( geometry.clone(), new THREE.LineBasicMaterial( {
					color: 0x00ff00,
					opacity: 0.35
				} ) );
				curve.mesh.castShadow = true;
				splines.centripetal = curve;

				curve = new THREE.CatmullRomCurve3( positions );
				curve.curveType = 'chordal';
				curve.mesh = new THREE.Line( geometry.clone(), new THREE.LineBasicMaterial( {
					color: 0x0000ff,
					opacity: 0.35
				} ) );
				curve.mesh.castShadow = true;
				splines.chordal = curve;

				for ( const k in splines ) {

					const spline = splines[ k ];
					scene.add( spline.mesh );

				}

				load( [ new THREE.Vector3( 289.76843686945404, 452.51481137238443, 56.10018915737797 ),
					new THREE.Vector3( - 53.56300074753207, 171.49711742836848, - 14.495472686253045 ),
					new THREE.Vector3( - 91.40118730204415, 176.4306956436485, - 6.958271935582161 ),
					new THREE.Vector3( - 383.785318791128, 491.1365363371675, 47.869296953772746 ) ] );

			}

			function addSplineObject( position ) {

				const material = new THREE.MeshLambertMaterial( { color: Math.random() * 0xffffff } );
				const object = new THREE.Mesh( geometry, material );

				if ( position ) {

					object.position.copy( position );

				} else {

					object.position.x = Math.random() * 1000 - 500;
					object.position.y = Math.random() * 600;
					object.position.z = Math.random() * 800 - 400;

				}

				object.castShadow = true;
				object.receiveShadow = true;
				scene.add( object );
				splineHelperObjects.push( object );
				return object;

			}

			function addPoint() {

				splinePointsLength ++;

				positions.push( addSplineObject().position );

				updateSplineOutline();

			}

			function removePoint() {

				if ( splinePointsLength <= 4 ) {

					return;

				}

				const point = splineHelperObjects.pop();
				splinePointsLength --;
				positions.pop();

				if ( transformControl.object === point ) transformControl.detach();
				scene.remove( point );

				updateSplineOutline();

			}

			function updateSplineOutline() {

				for ( const k in splines ) {

					const spline = splines[ k ];

					const splineMesh = spline.mesh;
					const position = splineMesh.geometry.attributes.position;

					for ( let i = 0; i < ARC_SEGMENTS; i ++ ) {

						const t = i / ( ARC_SEGMENTS - 1 );
						spline.getPoint( t, point );
						position.setXYZ( i, point.x, point.y, point.z );

					}

					position.needsUpdate = true;

				}

			}

			function exportSpline() {

				const strplace = [];

				for ( let i = 0; i < splinePointsLength; i ++ ) {

					const p = splineHelperObjects[ i ].position;
					strplace.push( `new THREE.Vector3(${p.x}, ${p.y}, ${p.z})` );

				}

				console.log( strplace.join( ',\n' ) );
				const code = '[' + ( strplace.join( ',\n\t' ) ) + ']';
				prompt( 'copy and paste code', code );

			}

			function load( new_positions ) {

				while ( new_positions.length > positions.length ) {

					addPoint();

				}

				while ( new_positions.length < positions.length ) {

					removePoint();

				}

				for ( let i = 0; i < positions.length; i ++ ) {

					positions[ i ].copy( new_positions[ i ] );

				}

				updateSplineOutline();

			}

			function animate() {

				requestAnimationFrame( animate );
				render();
				stats.update();

			}

			function render() {

				splines.uniform.mesh.visible = params.uniform;
				splines.centripetal.mesh.visible = params.centripetal;
				splines.chordal.mesh.visible = params.chordal;
				renderer.render( scene, camera );

			}

			function onPointerDown( event ) {

				onDownPosition.x = event.clientX;
				onDownPosition.y = event.clientY;

			}

			function onPointerUp() {

				onUpPosition.x = event.clientX;
				onUpPosition.y = event.clientY;

				if ( onDownPosition.distanceTo( onUpPosition ) === 0 ) transformControl.detach();

			}

			function onPointerMove( event ) {

				pointer.x = ( event.clientX / window.innerWidth ) * 2 - 1;
				pointer.y = - ( event.clientY / window.innerHeight ) * 2 + 1;

				raycaster.setFromCamera( pointer, camera );

				const intersects = raycaster.intersectObjects( splineHelperObjects );

				if ( intersects.length > 0 ) {

					const object = intersects[ 0 ].object;

					if ( object !== transformControl.object ) {

						transformControl.attach( object );

					}

				}

			}


		</script>

	</body>
</html>
