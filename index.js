/* globals */
import * as THREE from "../../../../three.js-dev/build/three.module.js";
import {
    WebGLRenderer,
    PerspectiveCamera,
    Scene,
    Mesh,
    PlaneBufferGeometry,
    ShadowMaterial,
    DirectionalLight,
    PCFSoftShadowMap,
    sRGBEncoding,
    Color,
    AmbientLight,
    Box3,
    LoadingManager,
    MathUtils,
} from 'three';
import { registerDragEvents } from './dragAndDrop.js';
import { STLLoader } from '../../../../three.js-dev/examples/jsm/loaders/STLLoader.js';
import { GLTFLoader } from '../../../../three.js-dev/examples/jsm/loaders/GLTFLoader.js';
import { ColladaLoader } from '../../../../three.js-dev/examples/jsm/loaders/ColladaLoader.js';
import { OBJLoader } from '../../../../three.js-dev/examples/jsm/loaders/OBJLoader.js';
import URDFManipulator from '../../src/urdf-manipulator-element.js';
import URDFLoader from '../../src/URDFLoader.js';
import { TWEEN } from '../../../../three.js-dev/examples/jsm/libs/tween.module.min.js';

customElements.define('urdf-viewer', URDFManipulator);

// declare these globally for the sake of the example.
// Hack to make the build work with webpack for now.
// TODO: Remove this once modules or parcel is being used
const viewer = document.querySelector('urdf-viewer');

const limitsToggle = document.getElementById('ignore-joint-limits');
const collisionToggle = document.getElementById('collision-toggle');
const radiansToggle = document.getElementById('radians-toggle');
const autocenterToggle = document.getElementById('autocenter-toggle');
const upSelect = document.getElementById('up-select');
const sliderList = document.querySelector('#controls ul');
const controlsel = document.getElementById('controls');
const controlsToggle = document.getElementById('toggle-controls');
const animToggle = document.getElementById('do-animate');
const togglefinger = document.getElementById('toggle-finger');
const addx = document.getElementById('addx');
const subx = document.getElementById('subx');
const addy = document.getElementById('addy');
const suby = document.getElementById('suby');
const addz = document.getElementById('addz');
const subz = document.getElementById('subz');
const showxyz = document.getElementById('showxyz');
const DEG2RAD = Math.PI / 180;
const RAD2DEG = 1 / DEG2RAD;
let sliders = {};
var arm6mesh;
var finger1mesh;
var finger2mesh;
var finger2mesh_flag=false;
var urdffile;
// Global Functions
const setColor = color => {

    document.body.style.backgroundColor = color;
    viewer.highlightColor = '#' + (new THREE.Color(0xffffff)).lerp(new THREE.Color(color), 0.35).getHexString();

};

// Events
// toggle checkbox
init();
	function init() {
		//載入物件
				const manager = new LoadingManager();
				const loader1 = new URDFLoader( manager );
				
				// The path to the URDF within the package OR absolute
				loader1.load('../../../../f7/urdf/f7.urdf', finger => {
					finger.scale.x = finger.scale.y = finger.scale.z = 0.5;
					// The robot is loaded!
					finger.position.x = 0;
					finger.position.z = 0.142;
					finger.rotation.y = 0 ;
					finger1mesh=finger;
					//finger.rotation.y = -Math.PI / 2;
					

					});
				const loader2 = new URDFLoader( manager );
				
				// The path to the URDF within the package OR absolute
				loader1.load('../../../../TMF06/urdf/TMF06.urdf', finger => {
					finger.scale.x = finger.scale.y = finger.scale.z = 1;
					// The robot is loaded!
					finger.position.x = -0.0;
					finger.position.y = -0.0 ;
					finger.position.z = 0.05;
					
					finger.rotation.y = -Math.PI/2 ;
					finger2mesh=finger;
					finger2mesh_flag=true;
					

					});
				const loader001 = new OBJLoader();
				// load a resource

				
				
				console.log(viewer.robot);
				const loader3 = new URDFLoader( manager );
				
				// The path to the URDF within the package OR absolute
				loader1.load('../../../staubli_tx2_90_support/urdf/tx2_90.urdf', object => {
					//object.scale.x = object.scale.y = object.scale.z = 1;
					// The robot is loaded!
					object.position.x =0.0;
					object.position.y = 0.8 ;
					object.position.z = 1;
					
					object.rotation.x = -Math.PI/2 ;
					viewer.scene.add( object );
					

					});
				const loader4 = new URDFLoader( manager );
				
				// The path to the URDF within the package OR absolute
				loader1.load('../../../fanuc_lrmate200id_support/urdf/lrmate200id.urdf', object => {
					//object.scale.x = object.scale.y = object.scale.z = 1;
					// The robot is loaded!
					object.position.x =0.0;
					object.position.y = 0.8 ;
					object.position.z = -1;
					
					object.rotation.x = -Math.PI/2 ;
					viewer.scene.add( object );
					

					});
					console.log("123");
					console.log(viewer.scene);
				loader1.load('../../../staubli_base/urdf/staubli_base.urdf', object => {
					//object.scale.x = object.scale.y = object.scale.z = 1;
					// The robot is loaded!
					object.position.x =-0.52;
					object.position.y = 0.66 ;
					object.position.z = 1.12;
					
					object.rotation.x = -Math.PI/2 ;
					object.rotation.z = Math.PI/2 ;
					viewer.scene.add( object );
					//urdffile=object;

					});	
					loader1.load('../../../TM_FANUC_BASE/urdf/TM_FANUC_BASE.urdf', object => {
					//object.scale.x = object.scale.y = object.scale.z = 1;
					// The robot is loaded!
					object.position.x =0;
					object.position.y = -0.22 ;
					object.position.z = -0.75;
					
					object.rotation.x = -Math.PI/2 ;
					object.rotation.z = Math.PI/2 ;
					viewer.scene.add( object );
					//urdffile=object;

					});	
					
					loader1.load('../../../cv13/urdf/cv13.urdf', object => {
					//object.scale.x = object.scale.y = object.scale.z = 1;
					// The robot is loaded!
					
					object.position.x =0.87;
					object.position.y = 0.6 ;
					object.position.z = -0.16;
					
					object.rotation.x = -Math.PI/2 ;
					
					//object.rotation.z = Math.PI/2 ;
				
				
					
					viewer.scene.add( object );
					
					});	
					loader1.load('../../../cv13/urdf/cv13.urdf', object => {
					//object.scale.x = object.scale.y = object.scale.z = 1;
					// The robot is loaded!
					
					object.position.x =0.78;
					object.position.y = 0.51 ;
					object.position.z = 0.56;
					
					object.rotation.x = -Math.PI/2 ;
					object.rotation.z = -Math.PI
					//object.rotation.z = Math.PI/2 ;
				
				
					
					viewer.scene.add( object );	
					urdffile=object;
					
					});	
			//viewer.robot.set
	
	}
			
				
togglefinger.addEventListener('click', () => {
	//alert(1);
	togglefinger.classList.toggle('checked');
	viewer.togglefinger = togglefinger.classList.contains('checked');


				if (viewer.togglefinger == true)
				{

					arm6mesh.add( finger1mesh );
					arm6mesh.remove(finger2mesh);
				}
				else
				{	
					arm6mesh.add( finger2mesh );
					arm6mesh.remove(finger1mesh);
				}
				viewer.renderer.render( viewer.scene, viewer.camera );
				
	
});

addx.addEventListener('click', () => {
    urdffile.position.x+=0.01;
	viewer.renderer.render( viewer.scene, viewer.camera );
});
addy.addEventListener('click', () => {
    urdffile.position.y+=0.01;
	viewer.renderer.render( viewer.scene, viewer.camera );
});
addz.addEventListener('click', () => {
    urdffile.position.z+=0.01;
	viewer.renderer.render( viewer.scene, viewer.camera );
});
subx.addEventListener('click', () => {
    urdffile.position.x-=0.01;
	viewer.renderer.render( viewer.scene, viewer.camera );
});
suby.addEventListener('click', () => {
    urdffile.position.y-=0.01;
	viewer.renderer.render( viewer.scene, viewer.camera );
});
subz.addEventListener('click', () => {
    urdffile.position.z-=0.01;
	viewer.renderer.render( viewer.scene, viewer.camera );
});
showxyz.addEventListener('click', () => {
    alert(urdffile.position.x+","+urdffile.position.y+","+urdffile.position.z);

});
limitsToggle.addEventListener('click', () => {
    limitsToggle.classList.toggle('checked');
    viewer.ignoreLimits = limitsToggle.classList.contains('checked');
});

radiansToggle.addEventListener('click', () => {
    radiansToggle.classList.toggle('checked');
    Object
        .values(sliders)
        .forEach(sl => sl.update());
});

collisionToggle.addEventListener('click', () => {
    collisionToggle.classList.toggle('checked');
    viewer.showCollision = collisionToggle.classList.contains('checked');
});

autocenterToggle.addEventListener('click', () => {
    autocenterToggle.classList.toggle('checked');
    viewer.noAutoRecenter = !autocenterToggle.classList.contains('checked');
});

upSelect.addEventListener('change', () => viewer.up = upSelect.value);

controlsToggle.addEventListener('click', () => controlsel.classList.toggle('hidden'));

// watch for urdf changes
viewer.addEventListener('urdf-change', () => {

    Object
        .values(sliders)
        .forEach(sl => sl.remove());
    sliders = {};

});

viewer.addEventListener('ignore-limits-change', () => {

    Object
        .values(sliders)
        .forEach(sl => sl.update());

});

viewer.addEventListener('angle-change', e => {

    if (sliders[e.detail]) sliders[e.detail].update();

});

viewer.addEventListener('joint-mouseover', e => {

    const j = document.querySelector(`li[joint-name="${ e.detail }"]`);
    if (j) j.setAttribute('robot-hovered', true);

});

viewer.addEventListener('joint-mouseout', e => {

    const j = document.querySelector(`li[joint-name="${ e.detail }"]`);
    if (j) j.removeAttribute('robot-hovered');

});

let originalNoAutoRecenter;
viewer.addEventListener('manipulate-start', e => {

    const j = document.querySelector(`li[joint-name="${ e.detail }"]`);
    if (j) {
        j.scrollIntoView({ block: 'nearest' });
        window.scrollTo(0, 0);
    }

    originalNoAutoRecenter = viewer.noAutoRecenter;
    viewer.noAutoRecenter = true;

});

viewer.addEventListener('manipulate-end', e => {

    viewer.noAutoRecenter = originalNoAutoRecenter;

});

// create the sliders
viewer.addEventListener('urdf-processed', () => {

    const r = viewer.robot;
		viewer.robot.traverse( function ( child ) {
						
		if (child.name=='arm6' ) {
			arm6mesh=child;
		}
	} );
	arm6mesh.add( finger2mesh );
	//console.log(r);
	//列出關節
    Object
        .keys(r.joints)
        .sort((a, b) => {

            const da = a.split(/[^\d]+/g).filter(v => !!v).pop();
            const db = b.split(/[^\d]+/g).filter(v => !!v).pop();

            if (da !== undefined && db !== undefined) {
                const delta = parseFloat(da) - parseFloat(db);
                if (delta !== 0) return delta;
            }

            if (a > b) return 1;
            if (b > a) return -1;
            return 0;

        })
        .map(key => r.joints[key])
        .forEach(joint => {

            const li = document.createElement('li');
            li.innerHTML =
            `
            <span title="${ joint.name }">${ joint.name }</span>
            <input type="range" value="0" step="0.0001"/>
            <input type="number" step="0.0001" />
            `;
            li.setAttribute('joint-type', joint.jointType);
            li.setAttribute('joint-name', joint.name);

            sliderList.appendChild(li);

            // update the joint display
            const slider = li.querySelector('input[type="range"]');
            const input = li.querySelector('input[type="number"]');
            li.update = () => {
                const degMultiplier = radiansToggle.classList.contains('checked') ? 1.0 : RAD2DEG;
                let angle = joint.angle;

                if (joint.jointType === 'revolute' || joint.jointType === 'continuous') {
                    angle *= degMultiplier;
                }

                if (Math.abs(angle) > 1) {
                    angle = angle.toFixed(1);
                } else {
                    angle = angle.toPrecision(2);
                }

                input.value = parseFloat(angle);

                // directly input the value
                slider.value = joint.angle;

                if (viewer.ignoreLimits || joint.jointType === 'continuous') {
                    slider.min = -6.28;
                    slider.max = 6.28;

                    input.min = -6.28 * degMultiplier;
                    input.max = 6.28 * degMultiplier;
                } else {
                    slider.min = joint.limit.lower;
                    slider.max = joint.limit.upper;

                    input.min = joint.limit.lower * degMultiplier;
                    input.max = joint.limit.upper * degMultiplier;
                }
            };

            switch (joint.jointType) {

                case 'continuous':
                case 'prismatic':
                case 'revolute':
                    break;
                default:
                    li.update = () => {};
                    input.remove();
                    slider.remove();

            }

            slider.addEventListener('input', () => {
                viewer.setJointValue(joint.name, slider.value);
                li.update();
            });

            input.addEventListener('change', () => {
                const degMultiplier = radiansToggle.classList.contains('checked') ? 1.0 : RAD2DEG;
                viewer.setJointValue(joint.name, input.value * degMultiplier);
                li.update();
            });

            li.update();

            sliders[joint.name] = li;

        });
		

});

document.addEventListener('WebComponentsReady', () => {
	
    viewer.loadMeshFunc = (path, manager, done) => {		
        const ext = path.split(/\./g).pop().toLowerCase();
        switch (ext) {

            case 'gltf':
            case 'glb':
                new GLTFLoader(manager).load(
                    path,
                    result => done(result.scene),
                    null,
                    err => done(null, err),
                );
                break;
            case 'obj':
                new OBJLoader(manager).load(
                    path,
                    result => done(result),
                    null,
                    err => done(null, err),
                );
                break;
            case 'dae':
                new ColladaLoader(manager).load(
                    path,
                    result => done(result.scene),
                    null,
                    err => done(null, err),
                );
                break;
            case 'stl':
                new STLLoader(manager).load(
                    path,
                    result => {
                        const material = new THREE.MeshPhongMaterial();
                        const mesh = new THREE.Mesh(result, material);
                        done(mesh);
                    },
                    null,
                    err => done(null, err),
                );
                break;

        }

    };

    document.querySelector('li[urdf]').dispatchEvent(new Event('click'));

    if (/javascript\/example\/bundle/i.test(window.location)) {
        viewer.package = '../urdf';
    }
	console.log('test');
	console.log(viewer);
    registerDragEvents(viewer, () => {
		
        setColor('#263238');
        animToggle.classList.remove('checked');
        updateList();
    });
	
});

// init 2D UI and animation
const updateAngles = () => {

    if (!viewer.setJointValue) return;

    // reset everything to 0 first
    const resetJointValues = viewer.angles;
    for (const name in resetJointValues) resetJointValues[name] = 0;
    viewer.setJointValues(resetJointValues);

    // animate the legs
    const time = Date.now() / 3e2;


        const offset =0;//  Math.PI / 3;
        const ratio = Math.max(0, Math.sin(time + offset));
		
        viewer.setJointValue(`j1`, THREE.MathUtils.lerp(30, 0, ratio) * DEG2RAD);
		viewer.setJointValue(`joint_1`, THREE.MathUtils.lerp(30, 0, ratio) * DEG2RAD);
		if (finger2mesh_flag)
		{
			for (let i = 1; i <= 5; i++) {
			finger1mesh.setJointValue(`finger${ i }_joint`, THREE.MathUtils.lerp(30, 0, ratio) * DEG2RAD);
			};
		}
		//console.log(ratio);
		if (finger2mesh_flag)
		{
			finger2mesh.setJointValue(`L_joint`, THREE.MathUtils.lerp(0, 0.03, ratio) );
			finger2mesh.setJointValue(`R_joint`, THREE.MathUtils.lerp(0, -0.03, ratio) );
		}


};

const updateLoop = () => {

    if (animToggle.classList.contains('checked')) {
        updateAngles();
    }

    requestAnimationFrame(updateLoop);

};

const updateList = () => {

    document.querySelectorAll('#urdf-options li[urdf]').forEach(el => {

        el.addEventListener('click', e => {

            const urdf = e.target.getAttribute('urdf');
            const color = e.target.getAttribute('color');

            viewer.up = 'Z';
            document.getElementById('up-select').value = viewer.up;
            viewer.urdf = urdf;
			console.log(urdf);
            animToggle.classList.add('checked');
            setColor(color);

        });

    });

};

updateList();

document.addEventListener('WebComponentsReady', () => {

    animToggle.addEventListener('click', () => animToggle.classList.toggle('checked'));

    // stop the animation if user tried to manipulate the model
    viewer.addEventListener('manipulate-start', e => animToggle.classList.remove('checked'));
    viewer.addEventListener('urdf-processed', e => updateAngles());
    updateLoop();
    viewer.camera.position.set(-5.5, 3.5, 5.5);
	console.log(viewer);
	
});
function setupTween() {

				const duration = THREE.MathUtils.randInt( 1000, 5000 );

				const target = {};
			
				const tweenParameters = {};
				//viewer.setJointValue(`j1`, THREE.MathUtils.lerp(30, 0, ratio) * DEG2RAD);
				//tweenParameters[]
				kinematicsTween = new TWEEN.Tween( tweenParameters ).to( target, duration ).easing( TWEEN.Easing.Quadratic.Out );

				kinematicsTween.onUpdate( function ( object ) {
					
					/*for ( const prop in kinematics.joints ) {

						if ( kinematics.joints.hasOwnProperty( prop ) ) {

							if ( ! kinematics.joints[ prop ].static ) {

								kinematics.setJointValue( prop, object[ prop ] );

							}

						}

					}*/

				} );

				kinematicsTween.start();

				//setTimeout( setupTween, duration );

			}
